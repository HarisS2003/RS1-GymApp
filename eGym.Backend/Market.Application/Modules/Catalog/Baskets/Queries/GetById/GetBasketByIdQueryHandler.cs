namespace Market.Application.Modules.Catalog.Baskets.Queries.GetById;

public sealed class GetBasketByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetBasketByIdQuery, GetBasketByIdQueryDto>
{
    public async Task<GetBasketByIdQueryDto> Handle(GetBasketByIdQuery request, CancellationToken ct)
    {
        var header = await ctx.Baskets.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new { x.Id, x.UserId })
            .FirstOrDefaultAsync(ct);

        if (header is null) throw new MarketNotFoundException($"Basket (ID={request.Id}) nije pronađen.");

        var items = await ctx.BasketItems.AsNoTracking()
            .Where(x => x.BasketId == request.Id)
            .Join(
                ctx.Products.AsNoTracking(),
                line => line.ProductId,
                p => p.Id,
                (line, p) => new { line, p })
            .Select(x => new BasketItemRowDto
            {
                Id = x.line.Id,
                ProductId = x.line.ProductId,
                ProductName = x.p.Name,
                UnitPrice = x.p.Price,
                Quantity = x.line.Quantity,
                LineTotal = Math.Round(x.p.Price * x.line.Quantity, 2, MidpointRounding.AwayFromZero)
            })
            .ToListAsync(ct);

        return new GetBasketByIdQueryDto
        {
            Id = header.Id,
            UserId = header.UserId,
            Items = items
        };
    }
}
