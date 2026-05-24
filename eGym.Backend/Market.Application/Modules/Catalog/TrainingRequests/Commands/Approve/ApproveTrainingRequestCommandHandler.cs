namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Approve;

public sealed class ApproveTrainingRequestCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ApproveTrainingRequestCommand, ApproveTrainingRequestResultDto>
{
    public async Task<ApproveTrainingRequestResultDto> Handle(
        ApproveTrainingRequestCommand request,
        CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var trainer = await ctx.Trainers
            .FirstOrDefaultAsync(x => x.UserId == userId, ct)
            ?? throw new ValidationException("Trainer profile not found for current user.");

        var booking = await ctx.TrainingRequests
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException("Training request not found.");

        if (booking.TrainerId != trainer.Id)
            throw new ValidationException("You can only manage your own training requests.");

        if (booking.Status != TrainingRequestStatus.Pending)
            throw new MarketBusinessRuleException("INVALID_STATUS", "Only pending requests can be approved.");

        var hasTraining = await ctx.Trainings.AnyAsync(
            x => x.TrainerId == booking.TrainerId && x.Date == booking.Date && x.StartTime == booking.StartTime,
            ct);

        if (hasTraining)
            throw new MarketBusinessRuleException("SLOT_TAKEN", "This time slot is no longer available.");

        var anchorTime = TrainingSlotRules.NormalizeTime(booking.StartTime);

        await DelayOtherSessionsAsync(
            booking.TrainerId,
            booking.Date,
            booking.Id,
            anchorTime,
            ct);

        var training = new TrainingEntity
        {
            TrainerId = booking.TrainerId,
            Type = TrainingType.Individual,
            Date = booking.Date,
            StartTime = anchorTime,
            Capacity = 1,
        };
        ctx.Trainings.Add(training);

        ctx.TrainingParticipants.Add(new TrainingParticipantEntity
        {
            Training = training,
            UserId = booking.UserId,
        });

        booking.Status = TrainingRequestStatus.Approved;
        booking.StartTime = anchorTime;

        await ctx.SaveChangesAsync(ct);

        return new ApproveTrainingRequestResultDto
        {
            TrainingRequestId = booking.Id,
            TrainingId = training.Id,
            Status = booking.Status,
        };
    }

    private async Task DelayOtherSessionsAsync(
        int trainerId,
        DateTime date,
        int excludeRequestId,
        TimeSpan anchorTime,
        CancellationToken ct)
    {
        var otherTrainings = await ctx.Trainings
            .Where(x => x.TrainerId == trainerId && x.Date == date)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(ct);

        foreach (var session in otherTrainings)
        {
            if (TrainingSlotRules.NormalizeTime(session.StartTime) == anchorTime)
                continue;

            var (newDate, newTime) = TrainingSlotRules.ShiftSession(session.Date, session.StartTime);
            session.Date = newDate;
            session.StartTime = newTime;
        }

        var otherRequests = await ctx.TrainingRequests
            .Where(x =>
                x.TrainerId == trainerId
                && x.Date == date
                && x.Id != excludeRequestId
                && (x.Status == TrainingRequestStatus.Pending || x.Status == TrainingRequestStatus.Approved))
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(ct);

        foreach (var request in otherRequests)
        {
            if (TrainingSlotRules.NormalizeTime(request.StartTime) == anchorTime)
                continue;

            var (newDate, newTime) = TrainingSlotRules.ShiftSession(request.Date, request.StartTime);
            request.Date = newDate;
            request.StartTime = newTime;
        }
    }
}
