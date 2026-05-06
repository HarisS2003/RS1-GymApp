namespace Market.Application.Modules.Catalog.Gyms.Queries.List;

public sealed class ListGymsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListGymsQuery, PageResult<ListGymsQueryDto>>
{
    public async Task<PageResult<ListGymsQueryDto>> Handle(ListGymsQuery request, CancellationToken ct)
    {
        var q = ctx.Gyms.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(searchTerm) || x.Address.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim().ToLower();
            q = q.Where(x => x.City.ToLower() == city);
        }

        var projectedQuery = q.Select(x => new ListGymsQueryDto
        {
            Id = x.Id,
            Name = x.Name,
            Address = x.Address,
            City = x.City
        });

        return await PageResult<ListGymsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
