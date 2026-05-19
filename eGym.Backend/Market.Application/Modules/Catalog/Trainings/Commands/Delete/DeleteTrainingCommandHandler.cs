namespace Market.Application.Modules.Catalog.Trainings.Commands.Delete;

public sealed class DeleteTrainingCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteTrainingCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTrainingCommand request, CancellationToken ct)
    {
        var training = await ctx.Trainings
            .Include(x => x.TrainingParticipants)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (training is null) throw new MarketNotFoundException("Training nije pronađen.");

        training.IsDeleted = true;
        training.ModifiedAtUtc = DateTime.UtcNow;

        foreach (var p in training.TrainingParticipants)
        {
            p.IsDeleted = true;
            p.ModifiedAtUtc = DateTime.UtcNow;
        }

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
