namespace Market.Application.Modules.Sales.Orders.Queries.ListWithItems;

public sealed class ListOrdersWithItemsQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
        : IRequestHandler<ListOrdersWithItemsQuery, PageResult<ListOrdersWithItemsQueryDto>>
{
    public async Task<PageResult<ListOrdersWithItemsQueryDto>> Handle(ListOrdersWithItemsQuery request, CancellationToken ct)
    {
        var q = context.Orders.AsNoTracking();
        if (!currentUser.IsAdmin)
        {
            q = q.Where(x => x.UserId == currentUser.UserId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x => x.Status.ToLower().Contains(searchTerm));
        }

        var projected = q.Select(x => new ListOrdersWithItemsQueryDto
        {
            Id = x.Id,
            UserId = x.UserId,
            Status = x.Status,
            TotalAmount = x.TotalAmount,
            Items = x.OrderItems.Select(i => new ListOrdersWithItemsQueryDtoItem
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        });

        return await PageResult<ListOrdersWithItemsQueryDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}

