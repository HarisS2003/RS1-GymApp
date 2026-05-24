namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Reject;

public sealed class RejectTrainingRequestCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<RejectTrainingRequestCommand, Unit>
{
    public async Task<Unit> Handle(RejectTrainingRequestCommand request, CancellationToken ct)
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
            throw new MarketBusinessRuleException("INVALID_STATUS", "Only pending requests can be rejected.");

        booking.Status = TrainingRequestStatus.Rejected;
        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
