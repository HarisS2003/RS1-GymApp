using Market.Application.Modules.Catalog.TrainingRequests;
using Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

namespace Market.Application.Modules.Catalog.Trainings.Commands.Create;

public sealed class CreateTrainingCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateTrainingCommand, int>
{
    public async Task<int> Handle(CreateTrainingCommand request, CancellationToken ct)
    {
        if (request.TrainerId <= 0) throw new ValidationException("TrainerId is required.");
        if (!Enum.IsDefined(typeof(TrainingType), request.Type))
            throw new ValidationException("Invalid training type.");
        if (request.Capacity <= 0) throw new ValidationException("Capacity must be positive.");

        if (!await ctx.Trainers.AnyAsync(x => x.Id == request.TrainerId, ct))
            throw new ValidationException("Invalid TrainerId.");

        var date = request.Date.Date;
        var startTime = TrainingSlotRules.NormalizeTime(request.StartTime);

        var sessions = await GetTrainerAvailableSlotsQueryHandler.LoadSessionsAsync(ctx, request.TrainerId, date, ct);
        if (!TrainingSlotRules.IsRentableSlot(sessions, startTime))
            throw new MarketBusinessRuleException("SLOT_TAKEN", "This time slot is no longer available.");

        await DelayOtherSessionsAsync(request.TrainerId, date, startTime, ct);

        var training = new TrainingEntity
        {
            TrainerId = request.TrainerId,
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
