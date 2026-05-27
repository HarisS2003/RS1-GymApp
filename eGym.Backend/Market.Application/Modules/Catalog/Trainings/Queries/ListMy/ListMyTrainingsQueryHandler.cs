namespace Market.Application.Modules.Catalog.Trainings.Queries.ListMy;

public sealed class ListMyTrainingsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ListMyTrainingsQuery, List<ListMyTrainingsQueryDto>>
{
    public async Task<List<ListMyTrainingsQueryDto>> Handle(ListMyTrainingsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return [];

        return await (
            from participant in ctx.TrainingParticipants.AsNoTracking()
            join training in ctx.Trainings.AsNoTracking() on participant.TrainingId equals training.Id
            join trainer in ctx.Trainers.AsNoTracking() on training.TrainerId equals trainer.Id
            join trainerUser in ctx.Users.AsNoTracking() on trainer.UserId equals trainerUser.Id
            where participant.UserId == userId
            orderby training.Date, training.StartTime
            select new ListMyTrainingsQueryDto
            {
                Id = training.Id,
                TrainerId = training.TrainerId,
                TrainerName = (trainerUser.FirstName + " " + trainerUser.LastName).Trim(),
                Type = training.Type,
                Description = training.Description,
                Date = training.Date,
                StartTime = training.StartTime,
                Capacity = training.Capacity,
                ParticipantsCount = training.TrainingParticipants.Count(),
            }
        ).ToListAsync(ct);
    }
}
