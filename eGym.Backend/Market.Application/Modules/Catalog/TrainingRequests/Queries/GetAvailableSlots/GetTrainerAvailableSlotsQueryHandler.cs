namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

public sealed class GetTrainerAvailableSlotsQueryHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<GetTrainerAvailableSlotsQuery, List<TrainerAvailableSlotDto>>
{
    public async Task<List<TrainerAvailableSlotDto>> Handle(
        GetTrainerAvailableSlotsQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.TrainerPublicId))
            throw new ValidationException("TrainerPublicId is required.");

        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var currentGymId = await ctx.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.GymId)
            .FirstOrDefaultAsync(ct);

        if (currentGymId == 0)
            throw new MarketNotFoundException("User not found.");

        var trainer = await ctx.Trainers.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.PublicId == request.TrainerPublicId && x.GymId == currentGymId && !x.IsDeleted,
                ct)
            ?? throw new MarketNotFoundException("Trainer not found.");

        var trainerId = trainer.Id;

        var day = request.Date.Date;
        if (day < DateTime.UtcNow.Date)
            throw new ValidationException("Cannot book a past date.");

        var sessions = await LoadSessionsAsync(ctx, trainerId, day, ct);
        var rentable = TrainingSlotRules.BuildRentableSlots(sessions);

        return rentable
            .Select(slot => new TrainerAvailableSlotDto
            {
                StartTime = slot.ToString(@"hh\:mm"),
                IsAvailable = true,
            })
            .ToList();
    }

    internal static async Task<List<TrainingSlotRules.SessionBlock>> LoadSessionsAsync(
        IAppDbContext ctx,
        int trainerId,
        DateTime day,
        CancellationToken ct)
    {
        var trainingStarts = await ctx.Trainings.AsNoTracking()
            .Where(x => x.TrainerId == trainerId && x.Date == day)
            .Select(x => x.StartTime)
            .ToListAsync(ct);

        var pendingStarts = await ctx.TrainingRequests.AsNoTracking()
            .Where(x =>
                x.TrainerId == trainerId
                && x.Date == day
                && x.Status == TrainingRequestStatus.Pending)
            .Select(x => x.StartTime)
            .ToListAsync(ct);

        var sessions = new List<TrainingSlotRules.SessionBlock>();
        sessions.AddRange(trainingStarts.Select(t => new TrainingSlotRules.SessionBlock(
            TrainingSlotRules.NormalizeTime(t),
            IsApproved: true)));
        sessions.AddRange(pendingStarts.Select(t => new TrainingSlotRules.SessionBlock(
            TrainingSlotRules.NormalizeTime(t),
            IsApproved: false)));

        return sessions;
    }
}
