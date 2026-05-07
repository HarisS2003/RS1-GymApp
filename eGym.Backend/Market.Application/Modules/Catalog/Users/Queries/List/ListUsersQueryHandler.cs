namespace Market.Application.Modules.Catalog.Users.Queries.List;

public sealed class ListUsersQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListUsersQuery, PageResult<ListUsersQueryDto>>
{
    public async Task<PageResult<ListUsersQueryDto>> Handle(ListUsersQuery request, CancellationToken ct)
    {
        var q = ctx.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x =>
                x.FirstName.ToLower().Contains(searchTerm)
                || x.LastName.ToLower().Contains(searchTerm)
                || x.Email.ToLower().Contains(searchTerm));
        }

        if (request.GymId is int gymId)
            q = q.Where(x => x.GymId == gymId);

        if (request.RoleId is int roleId)
            q = q.Where(x => x.RoleId == roleId);

        var projectedQuery = q.Select(x => new ListUsersQueryDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            RoleId = x.RoleId,
            GymId = x.GymId
        });

        return await PageResult<ListUsersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
