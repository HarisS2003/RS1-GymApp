namespace Market.Application.Modules.Catalog.Users.Queries.GetById;

public sealed class GetUserByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryDto>
{
    public async Task<GetUserByIdQueryDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var dto = await ctx.Users.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetUserByIdQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                RoleId = x.RoleId,
                GymId = x.GymId
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"User (ID={request.Id}) nije pronađen.");
    }
}
