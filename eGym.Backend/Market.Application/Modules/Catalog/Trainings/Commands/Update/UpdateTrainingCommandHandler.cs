namespace Market.Application.Modules.Catalog.Trainings.Commands.Update;

public sealed class UpdateTrainingCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateTrainingCommand, Unit>
{
    public async Task<Unit> Handle(UpdateTrainingCommand request, CancellationToken ct)
    {
        var entity = await ctx.Trainings.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) throw new MarketNotFoundException($"Training (ID={request.Id}) nije pronađen.");

        if (request.TrainerId <= 0) throw new ValidationException("TrainerId is required.");
        if (!Enum.IsDefined(typeof(TrainingType), request.Type))
            throw new ValidationException("Invalid training type.");
        if (request.Capacity <= 0) throw new ValidationException("Capacity must be positive.");

        if (!await ctx.Trainers.AnyAsync(x => x.Id == request.TrainerId, ct))
            throw new ValidationException("Invalid TrainerId.");

        var activeParticipants = await ctx.TrainingParticipants.CountAsync(
            x => x.TrainingId == request.Id,
            ct);
        if (activeParticipants > request.Capacity)
            throw new ValidationException("Capacity cannot be less than current number of participants.");

        entity.TrainerId = request.TrainerId;
        entity.Type = request.Type;
        entity.Date = request.Date.Date;
        entity.StartTime = request.StartTime;
        entity.Capacity = request.Capacity;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
