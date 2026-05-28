namespace Market.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var normalized = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ValidationException("Name is required.");

        var categoryName = request.CategoryName.Trim();
        if (string.IsNullOrWhiteSpace(categoryName)) throw new ValidationException("CategoryName is required.");

        var variants = ProductVariantSync.NormalizeVariants(request.Variants);

        if (!await ctx.Gyms.AnyAsync(x => x.Id == request.GymId && !x.IsDeleted, ct))
            throw new ValidationException("Invalid GymId.");

        var activeExists = await ctx.Products.AnyAsync(
            x => x.Name == normalized && x.GymId == request.GymId, ct);
        if (activeExists) throw new MarketConflictException("Name already exists in gym.");

        var deleted = await ctx.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Name == normalized && x.GymId == request.GymId && x.IsDeleted, ct);

        if (deleted is not null)
        {
            deleted.IsDeleted = false;
            deleted.CategoryName = categoryName;
            deleted.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            deleted.ModifiedAtUtc = DateTime.UtcNow;

            if (variants.Count > 0)
                await ProductVariantSync.ApplyVariantsAsync(ctx, deleted, variants, ct);
            else
            {
                deleted.Price = request.Price;
                deleted.StockQuantity = request.StockQuantity;
                await EnsureDefaultVariantAsync(deleted.Id, deleted.Price, deleted.StockQuantity, ct);
            }

            await ctx.SaveChangesAsync(ct);
            return deleted.Id;
        }

        decimal price = request.Price;
        var stockQuantity = request.StockQuantity;

        if (variants.Count > 0)
        {
            ProductVariantSync.ValidateVariants(variants);
            (price, stockQuantity) = ProductVariantSync.Aggregate(variants);
        }
        else
        {
            if (price < 0) throw new ValidationException("Price must be zero or positive.");
            if (stockQuantity < 0) throw new ValidationException("StockQuantity must be zero or positive.");
        }

        var product = new ProductEntity
        {
            Name = normalized,
            CategoryName = categoryName,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Price = price,
            StockQuantity = stockQuantity,
            GymId = request.GymId,
        };

        ctx.Products.Add(product);
        await ctx.SaveChangesAsync(ct);

        if (variants.Count > 0)
            await ProductVariantSync.ApplyVariantsAsync(ctx, product, variants, ct);
        else
            await EnsureDefaultVariantAsync(product.Id, product.Price, product.StockQuantity, ct);

        await ctx.SaveChangesAsync(ct);
        return product.Id;
    }

    private async Task EnsureDefaultVariantAsync(int productId, decimal price, int stockQuantity, CancellationToken ct)
    {
        var hasVariant = await ctx.ProductVariants.AnyAsync(v => v.ProductId == productId, ct);
        if (hasVariant) return;

        ctx.ProductVariants.Add(new ProductVariantEntity
        {
            ProductId = productId,
            Size = "Standard",
            Color = "Standard",
            Price = price,
            StockQuantity = stockQuantity,
        });
    }
}
