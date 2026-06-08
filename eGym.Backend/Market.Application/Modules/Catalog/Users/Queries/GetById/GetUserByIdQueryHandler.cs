namespace Market.Application.Modules.Catalog.Users.Queries.GetById;

public sealed class GetUserByIdQueryHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryDto>
{
    public async Task<GetUserByIdQueryDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var dto = await ctx.Users.AsNoTracking()
            .Where(x => x.PublicId == request.PublicId && x.GymId == currentGymId)
            .Select(x => new GetUserByIdQueryDto
            {
                PublicId = x.PublicId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                RoleId = x.RoleId,
                GymId = x.GymId
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException("User not found.");
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
