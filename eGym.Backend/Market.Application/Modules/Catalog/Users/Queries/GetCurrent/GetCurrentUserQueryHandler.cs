using Market.Application.Modules.Catalog.Users.Queries.GetById;

namespace Market.Application.Modules.Catalog.Users.Queries.GetCurrent;

public sealed class GetCurrentUserQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetCurrentUserQuery, GetUserByIdQueryDto>
{
    public async Task<GetUserByIdQueryDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketNotFoundException("User not found.");

        var dto = await ctx.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new GetUserByIdQueryDto
            {
                PublicId = x.PublicId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                RoleId = x.RoleId,
                GymId = x.GymId,
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException("User not found.");
    }
}
