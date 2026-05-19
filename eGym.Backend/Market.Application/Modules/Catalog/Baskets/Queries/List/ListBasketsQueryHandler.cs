namespace Market.Application.Modules.Catalog.Baskets.Queries.List;

public sealed class ListBasketsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListBasketsQuery, PageResult<ListBasketsQueryDto>>
{
    public async Task<PageResult<ListBasketsQueryDto>> Handle(ListBasketsQuery request, CancellationToken ct)
    {
        var q = ctx.Baskets.AsNoTracking();

        if (request.UserId is int userId)
            q = q.Where(x => x.UserId == userId);

        var projectedQuery = q.Select(x => new ListBasketsQueryDto
        {
            Id = x.Id,
            UserId = x.UserId,
            ItemsCount = x.BasketItems.Count()
        });

        return await PageResult<ListBasketsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
