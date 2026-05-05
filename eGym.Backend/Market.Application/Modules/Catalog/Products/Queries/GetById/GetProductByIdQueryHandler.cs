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
                Price = x.Price,
                StockQuantity = x.StockQuantity,
                GymId = x.GymId
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Product (ID={request.Id}) nije pronađen.");
    }
}
