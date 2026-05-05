namespace Market.Application.Modules.Catalog.Products.Queries.List;

public sealed class ListProductsQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListProductsQuery, PageResult<ListProductsQueryDto>>
{
    public async Task<PageResult<ListProductsQueryDto>> Handle(ListProductsQuery request, CancellationToken ct)
    {
        var q = ctx.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(searchTerm));
        }

        var projectedQuery = q.Select(x => new ListProductsQueryDto
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            StockQuantity = x.StockQuantity,
            GymId = x.GymId
        });

        return await PageResult<ListProductsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}

