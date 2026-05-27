namespace Market.Application.Modules.Catalog.Trainings.Queries.List;

public sealed class ListTrainingsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListTrainingsQuery, PageResult<ListTrainingsQueryDto>>
{
    public async Task<PageResult<ListTrainingsQueryDto>> Handle(ListTrainingsQuery request, CancellationToken ct)
    {
        var q = ctx.Trainings.AsNoTracking();

        if (request.TrainerId is int trainerId)
            q = q.Where(x => x.TrainerId == trainerId);

        if (request.Type is TrainingType type)
            q = q.Where(x => x.Type == type);

        if (request.DateFrom is DateTime from)
            q = q.Where(x => x.Date >= from.Date);

        if (request.DateTo is DateTime to)
            q = q.Where(x => x.Date <= to.Date);

        var projectedQuery = q.Select(x => new ListTrainingsQueryDto
        {
            Id = x.Id,
            TrainerId = x.TrainerId,
            TrainerName = x.Trainer!.User!.FirstName + " " + x.Trainer.User.LastName,
            Type = x.Type,
            Description = x.Description,
            Date = x.Date,
            StartTime = x.StartTime,
            Capacity = x.Capacity,
            ParticipantsCount = x.TrainingParticipants.Count()
        });

        return await PageResult<ListTrainingsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
