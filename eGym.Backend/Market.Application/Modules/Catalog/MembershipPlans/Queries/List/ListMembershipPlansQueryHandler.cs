namespace Market.Application.Modules.Catalog.MembershipPlans.Queries.List;

public sealed class ListMembershipPlansQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListMembershipPlansQuery, PageResult<ListMembershipPlansQueryDto>>
{
    public async Task<PageResult<ListMembershipPlansQueryDto>> Handle(ListMembershipPlansQuery request, CancellationToken ct)
    {
        var q = ctx.MembershipPlans.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(searchTerm));
        }

        if (request.GymId is int gymId)
            q = q.Where(x => x.GymId == gymId);

        var projectedQuery = q.Select(x => new ListMembershipPlansQueryDto
        {
            Id = x.Id,
            Name = x.Name,
            DurationDays = x.DurationDays,
            Price = x.Price,
            DiscountPercentage = x.DiscountPercentage,
            GymId = x.GymId
        });

        return await PageResult<ListMembershipPlansQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
