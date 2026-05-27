using Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Create;

public sealed class CreateTrainingRequestCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<CreateTrainingRequestCommand, CreateTrainingRequestResultDto>
{
    public async Task<CreateTrainingRequestResultDto> Handle(
        CreateTrainingRequestCommand request,
        CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        if (request.TrainerId <= 0)
            throw new ValidationException("TrainerId is required.");

        var day = request.Date.Date;
        if (day < DateTime.UtcNow.Date)
            throw new ValidationException("Cannot book a past date.");

        var startTime = TrainingSlotRules.NormalizeTime(request.StartTime);

        var trainer = await ctx.Trainers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.TrainerId, ct)
            ?? throw new MarketNotFoundException("Trainer not found.");

        if (trainer.UserId == userId)
            throw new ValidationException("You cannot book yourself.");

        var user = await ctx.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, ct)
            ?? throw new MarketNotFoundException("User not found.");

        if (user.GymId != trainer.GymId)
            throw new ValidationException("Trainer does not belong to your gym.");

        var today = DateTime.UtcNow.Date;
        var hasActiveMembership = await ctx.UserMemberships.AsNoTracking()
            .AnyAsync(m => m.UserId == userId && m.EndDate >= today, ct);

        if (!hasActiveMembership)
            throw new MarketBusinessRuleException(
                "MEMBERSHIP_REQUIRED",
                "Active membership plan is required to book a trainer.");

        var sessions = await GetTrainerAvailableSlotsQueryHandler.LoadSessionsAsync(
            ctx,
            request.TrainerId,
            day,
            ct);

        if (!TrainingSlotRules.IsRentableSlot(sessions, startTime))
            throw new MarketBusinessRuleException("SLOT_TAKEN", "This time slot is no longer available.");

        var entity = new TrainingRequestEntity
        {
            UserId = userId,
            TrainerId = request.TrainerId,
            Date = day,
            StartTime = startTime,
            Status = TrainingRequestStatus.Pending,
        };

        ctx.TrainingRequests.Add(entity);
        await ctx.SaveChangesAsync(ct);

        return new CreateTrainingRequestResultDto
        {
            TrainingRequestId = entity.Id,
            TrainerId = entity.TrainerId,
            Date = entity.Date,
            StartTime = entity.StartTime,
            Status = entity.Status,
        };
    }
}
