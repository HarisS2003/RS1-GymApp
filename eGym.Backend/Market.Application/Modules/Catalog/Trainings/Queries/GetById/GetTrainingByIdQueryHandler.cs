namespace Market.Application.Modules.Catalog.Trainings.Queries.GetById;

public sealed class GetTrainingByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetTrainingByIdQuery, GetTrainingByIdQueryDto>
{
    public async Task<GetTrainingByIdQueryDto> Handle(GetTrainingByIdQuery request, CancellationToken ct)
    {
        var dto = await ctx.Trainings.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetTrainingByIdQueryDto
            {
                Id = x.Id,
                TrainerId = x.TrainerId,
                Type = x.Type,
                Date = x.Date,
                StartTime = x.StartTime,
                Capacity = x.Capacity,
                ParticipantsCount = x.TrainingParticipants.Count()
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Training (ID={request.Id}) nije pronađen.");
    }
}
