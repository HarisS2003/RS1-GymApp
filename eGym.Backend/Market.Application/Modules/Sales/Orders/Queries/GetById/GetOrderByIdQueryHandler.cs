namespace Market.Application.Modules.Sales.Orders.Queries.GetById;

public class GetOrderByIdQueryHandler(IAppDbContext context, IAppCurrentUser currentUser) : IRequestHandler<GetOrderByIdQuery, GetOrderByIdQueryDto>
{
    public async Task<GetOrderByIdQueryDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var q = context.Orders.Where(x => x.Id == request.Id).AsNoTracking();
        if (!currentUser.IsAdmin)
        {
            q = q.Where(x => x.UserId == currentUser.UserId);
        }

        var dto = await q.Select(x => new GetOrderByIdQueryDto
        {
            Id = x.Id,
            UserId = x.UserId,
            Status = x.Status,
            TotalAmount = x.TotalAmount,
            Items = x.OrderItems.Select(i => new GetByIdOrderQueryDtoItem
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        }).FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Order (ID={request.Id}) nije pronađen.");
    }
}
