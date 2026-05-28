namespace Market.Application.Modules.Catalog.Products;

internal static class ProductVariantSync
{
    public static IReadOnlyList<ProductVariantCommandDto> NormalizeVariants(
        IReadOnlyList<ProductVariantCommandDto>? variants)
    {
        if (variants is null || variants.Count == 0)
            return Array.Empty<ProductVariantCommandDto>();

        return variants
            .Select(v => new ProductVariantCommandDto
            {
                Id = v.Id,
                Size = ProductVariantNormalization.NormalizeSize(v.Size),
                Color = v.Color.Trim(),
                Price = v.Price,
                StockQuantity = v.StockQuantity,
            })
            .Where(v => !string.IsNullOrWhiteSpace(v.Size) && !string.IsNullOrWhiteSpace(v.Color))
            .ToList();
    }

    public static void ValidateVariants(IReadOnlyList<ProductVariantCommandDto> variants)
    {
        if (variants.Count == 0)
            throw new ValidationException("At least one product variant is required.");

        var duplicate = variants
            .GroupBy(
                v => $"{ProductVariantNormalization.CanonicalSizeKey(v.Size)}|{ProductVariantNormalization.CanonicalColorKey(v.Color)}",
                StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicate is not null)
            throw new ValidationException(
                "Duplicate variant: same size and flavor (e.g. 450 and 450g count as one size).");

        foreach (var variant in variants)
        {
            if (variant.Price < 0)
                throw new ValidationException("Variant price must be zero or positive.");

            if (variant.StockQuantity < 0)
                throw new ValidationException("Variant stock must be zero or positive.");
        }
    }

    public static (decimal Price, int StockQuantity) Aggregate(IReadOnlyList<ProductVariantCommandDto> variants)
        => (variants.Min(v => v.Price), variants.Sum(v => v.StockQuantity));

    public static async Task ApplyVariantsAsync(
        IAppDbContext ctx,
        ProductEntity product,
        IReadOnlyList<ProductVariantCommandDto> variants,
        CancellationToken ct)
    {
        ValidateVariants(variants);

        var existing = await ctx.ProductVariants
            .Where(v => v.ProductId == product.Id)
            .ToListAsync(ct);

        var incomingIds = variants
            .Where(v => v.Id is > 0)
            .Select(v => v.Id!.Value)
            .ToHashSet();

        foreach (var row in existing.Where(v => !incomingIds.Contains(v.Id)))
            ctx.ProductVariants.Remove(row);

        foreach (var dto in variants)
        {
            if (dto.Id is int id && id > 0)
            {
                var entity = existing.FirstOrDefault(v => v.Id == id)
                    ?? throw new ValidationException($"Variant (ID={id}) not found on this product.");

                entity.Size = ProductVariantNormalization.NormalizeSize(dto.Size);
                entity.Color = dto.Color.Trim();
                entity.Price = dto.Price;
                entity.StockQuantity = dto.StockQuantity;
                continue;
            }

            ctx.ProductVariants.Add(new ProductVariantEntity
            {
                ProductId = product.Id,
                Size = ProductVariantNormalization.NormalizeSize(dto.Size),
                Color = dto.Color.Trim(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
            });
        }

        var (price, stock) = Aggregate(variants);
        product.Price = price;
        product.StockQuantity = stock;
    }
}
