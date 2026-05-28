using Market.Application.Modules.Catalog.Products;

namespace Market.Application.Modules.Catalog.Products.Queries.GetById;

public sealed class GetProductByIdQueryHandler(IAppDbContext ctx) : IRequestHandler<GetProductByIdQuery, GetProductByIdQueryDto>
{
    public async Task<GetProductByIdQueryDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var dto = await ctx.Products.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetProductByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                CategoryName = x.CategoryName,
                Description = x.Description,
                Price = x.Price,
                StockQuantity = x.StockQuantity,
                GymId = x.GymId,
                IsEnabled = true,
                ProductVariants = x.ProductVariants
                    .OrderBy(v => v.Size)
                    .ThenBy(v => v.Color)
                    .Select(v => new ProductVariantQueryDto
                    {
                        Id = v.Id,
                        ProductId = v.ProductId,
                        Size = v.Size,
                        Color = v.Color,
                        Price = v.Price,
                        StockQuantity = v.StockQuantity,
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Product (ID={request.Id}) nije pronađen.");
    }
}
