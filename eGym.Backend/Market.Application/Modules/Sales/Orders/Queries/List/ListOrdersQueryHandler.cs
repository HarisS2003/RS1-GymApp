namespace Market.Application.Modules.Sales.Orders.Queries.List;

public sealed class ListOrdersQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
        : IRequestHandler<ListOrdersQuery, PageResult<ListOrdersQueryDto>>
{
    public async Task<PageResult<ListOrdersQueryDto>> Handle(ListOrdersQuery request, CancellationToken ct)
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

        var projected = q.Select(x => new ListOrdersQueryDto
        {
            Id = x.Id,
            UserId = x.UserId,
            Status = x.Status,
            TotalAmount = x.TotalAmount
        });

        return await PageResult<ListOrdersQueryDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}

