namespace Market.Application.Modules.Catalog.Trainers.Queries.GetById;

public sealed class GetTrainerByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetTrainerByIdQuery, GetTrainerByIdQueryDto>
{
    public async Task<GetTrainerByIdQueryDto> Handle(GetTrainerByIdQuery request, CancellationToken ct)
    {
        var dto = await ctx.Trainers.AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new GetTrainerByIdQueryDto
            {
                Id = x.Id,
                UserId = x.UserId,
                GymId = x.GymId,
                Bio = x.Bio,
                ExperienceYears = x.ExperienceYears
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Trainer (ID={request.Id}) nije pronađen.");
    }
}
