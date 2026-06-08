namespace Market.Application.Modules.Catalog.Users.Queries.List;

public sealed class ListUsersQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ListUsersQuery, PageResult<ListUsersQueryDto>>
{
    public async Task<PageResult<ListUsersQueryDto>> Handle(ListUsersQuery request, CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var q = ctx.Users.AsNoTracking()
            .Where(x => x.GymId == currentGymId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x =>
                x.FirstName.ToLower().Contains(searchTerm)
                || x.LastName.ToLower().Contains(searchTerm)
                || x.Email.ToLower().Contains(searchTerm));
        }

        if (request.RoleId is int roleId)
            q = q.Where(x => x.RoleId == roleId);

        var projectedQuery = q.Select(x => new ListUsersQueryDto
        {
            PublicId = x.PublicId,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            RoleId = x.RoleId,
            GymId = x.GymId
        });

        return await PageResult<ListUsersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }

    private async Task<int> GetCurrentGymIdAsync(CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var gymId = await ctx.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.GymId)
            .FirstOrDefaultAsync(ct);

        return gymId == 0
            ? throw new MarketNotFoundException("User not found.")
            : gymId;
    }
}
