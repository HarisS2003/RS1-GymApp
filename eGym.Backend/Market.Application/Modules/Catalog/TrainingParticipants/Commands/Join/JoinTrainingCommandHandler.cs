namespace Market.Application.Modules.Catalog.TrainingParticipants.Commands.Join;

public sealed class JoinTrainingCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<JoinTrainingCommand, Unit>
{
    public async Task<Unit> Handle(JoinTrainingCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var training = await ctx.Trainings
            .Include(x => x.Trainer)
            .FirstOrDefaultAsync(x => x.Id == request.TrainingId, ct)
            ?? throw new MarketNotFoundException("Training not found.");

        if (training.Type != TrainingType.Group)
            throw new ValidationException("Only group trainings can be joined.");

        var startsAt = training.Date.Date.Add(training.StartTime);
        if (startsAt <= DateTime.Now)
            throw new MarketBusinessRuleException("TRAINING_EXPIRED", "This training has already finished.");

        var user = await ctx.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, ct)
            ?? throw new MarketNotFoundException("User not found.");

        if (training.Trainer?.GymId != user.GymId)
            throw new ValidationException("Training does not belong to your gym.");

        if (training.Trainer.UserId == userId)
            throw new ValidationException("Trainer cannot join own group training.");

        var alreadyJoined = await ctx.TrainingParticipants.AnyAsync(
            x => x.TrainingId == training.Id && x.UserId == userId,
            ct);

        if (alreadyJoined)
            throw new MarketBusinessRuleException("ALREADY_JOINED", "You already joined this training.");

        var participants = await ctx.TrainingParticipants.CountAsync(x => x.TrainingId == training.Id, ct);
        if (participants >= training.Capacity)
            throw new MarketBusinessRuleException("TRAINING_FULL", "This group training is full.");

        ctx.TrainingParticipants.Add(new TrainingParticipantEntity
        {
            TrainingId = training.Id,
            UserId = userId,
        });

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
