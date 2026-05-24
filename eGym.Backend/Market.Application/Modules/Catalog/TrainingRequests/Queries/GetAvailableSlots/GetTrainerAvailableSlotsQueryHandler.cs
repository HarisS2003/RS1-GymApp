namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

public sealed class GetTrainerAvailableSlotsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetTrainerAvailableSlotsQuery, List<TrainerAvailableSlotDto>>
{
    public async Task<List<TrainerAvailableSlotDto>> Handle(
        GetTrainerAvailableSlotsQuery request,
        CancellationToken ct)
    {
        if (request.TrainerId <= 0)
            throw new ValidationException("TrainerId is required.");

        var day = request.Date.Date;
        if (day < DateTime.UtcNow.Date)
            throw new ValidationException("Cannot book a past date.");

        if (!await ctx.Trainers.AnyAsync(x => x.Id == request.TrainerId, ct))
            throw new MarketNotFoundException("Trainer not found.");

        var sessions = await LoadSessionsAsync(ctx, request.TrainerId, day, ct);
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
