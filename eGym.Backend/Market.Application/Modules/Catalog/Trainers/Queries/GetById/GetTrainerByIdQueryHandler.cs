namespace Market.Application.Modules.Catalog.Trainers.Queries.GetById;

public sealed class GetTrainerByIdQueryHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<GetTrainerByIdQuery, GetTrainerByIdQueryDto>
{
    public async Task<GetTrainerByIdQueryDto> Handle(GetTrainerByIdQuery request, CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var dto = await (
            from t in ctx.Trainers.AsNoTracking()
            join u in ctx.Users.AsNoTracking() on t.UserId equals u.Id
            where t.PublicId == request.PublicId && t.GymId == currentGymId && !t.IsDeleted
            select new GetTrainerByIdQueryDto
            {
                PublicId = t.PublicId,
                UserPublicId = u.PublicId,
                GymId = t.GymId,
                Bio = t.Bio,
                ExperienceYears = t.ExperienceYears
            }
        ).FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException("Trainer not found.");
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
