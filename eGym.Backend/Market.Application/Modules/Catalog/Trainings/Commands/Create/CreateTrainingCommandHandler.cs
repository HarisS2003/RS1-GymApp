using Market.Application.Modules.Catalog.TrainingRequests;
using Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

namespace Market.Application.Modules.Catalog.Trainings.Commands.Create;

public sealed class CreateTrainingCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<CreateTrainingCommand, int>
{
    public async Task<int> Handle(CreateTrainingCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.TrainerPublicId))
            throw new ValidationException("TrainerPublicId is required.");
        if (!Enum.IsDefined(typeof(TrainingType), request.Type))
            throw new ValidationException("Invalid training type.");
        if (request.Capacity <= 0) throw new ValidationException("Capacity must be positive.");

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

        var date = request.Date.Date;
        var startTime = TrainingSlotRules.NormalizeTime(request.StartTime);

        var sessions = await GetTrainerAvailableSlotsQueryHandler.LoadSessionsAsync(ctx, trainerId, date, ct);
        if (!TrainingSlotRules.IsRentableSlot(sessions, startTime))
            throw new MarketBusinessRuleException("SLOT_TAKEN", "This time slot is no longer available.");

        await DelayOtherSessionsAsync(trainerId, date, startTime, ct);

        var training = new TrainingEntity
        {
            TrainerId = trainerId,
            Type = request.Type,
            Description = request.Description?.Trim(),
            Date = date,
            StartTime = startTime,
            Capacity = request.Capacity
        };

        ctx.Trainings.Add(training);
        await ctx.SaveChangesAsync(ct);
        return training.Id;
    }

    private async Task DelayOtherSessionsAsync(int trainerId, DateTime date, TimeSpan anchorTime, CancellationToken ct)
    {
        var trainings = await ctx.Trainings
            .Where(x => x.TrainerId == trainerId && x.Date == date)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(ct);

        foreach (var training in trainings)
        {
            if (TrainingSlotRules.NormalizeTime(training.StartTime) == anchorTime)
                continue;

            var (newDate, newTime) = TrainingSlotRules.ShiftSession(training.Date, training.StartTime);
            training.Date = newDate;
            training.StartTime = newTime;
        }

        var requests = await ctx.TrainingRequests
            .Where(x =>
                x.TrainerId == trainerId
                && x.Date == date
                && (x.Status == TrainingRequestStatus.Pending || x.Status == TrainingRequestStatus.Approved))
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(ct);

        foreach (var request in requests)
        {
            if (TrainingSlotRules.NormalizeTime(request.StartTime) == anchorTime)
                continue;

            var (newDate, newTime) = TrainingSlotRules.ShiftSession(request.Date, request.StartTime);
            request.Date = newDate;
            request.StartTime = newTime;
        }
    }
}
