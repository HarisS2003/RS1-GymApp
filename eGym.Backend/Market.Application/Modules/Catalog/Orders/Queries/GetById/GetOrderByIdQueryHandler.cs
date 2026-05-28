namespace Market.Application.Modules.Catalog.Orders.Queries.GetById;

public sealed class GetOrderByIdQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetOrderByIdQuery, GetOrderByIdQueryDto>
{
    public async Task<GetOrderByIdQueryDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var q = ctx.Orders.AsNoTracking().Where(x => x.Id == request.Id && !x.IsDeleted);
        if (!currentUser.IsAdmin)
            q = q.Where(x => x.UserId == currentUser.UserId);

        var dto = await q.Select(x => new GetOrderByIdQueryDto
        {
            Id = x.Id,
            UserId = x.UserId,
            Status = x.Status,
            TotalAmount = x.TotalAmount,
            Items = x.OrderItems
                .Where(i => !i.IsDeleted)
                .Select(i => new GetOrderByIdQueryDtoItem
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product!.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                })
                .ToList()
        }).FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Order (ID={request.Id}) nije pronađen.");
    }
}
