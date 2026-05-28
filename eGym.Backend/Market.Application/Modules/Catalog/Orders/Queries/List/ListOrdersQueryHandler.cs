namespace Market.Application.Modules.Catalog.Orders.Queries.List;

public sealed class ListOrdersQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ListOrdersQuery, PageResult<ListOrdersQueryDto>>
{
    public async Task<PageResult<ListOrdersQueryDto>> Handle(ListOrdersQuery request, CancellationToken ct)
    {
        var q = ctx.Orders.AsNoTracking().Where(x => !x.IsDeleted);

        if (!currentUser.IsAdmin)
            q = q.Where(x => x.UserId == currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            q = q.Where(x => x.Status.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var status = request.Status.Trim();
            q = q.Where(x => x.Status == status);
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
