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

        var training = new TrainingEntity
        {
            TrainerId = request.TrainerId,
            Type = request.Type,
            Date = request.Date.Date,
            StartTime = request.StartTime,
            Capacity = request.Capacity
        };

        ctx.Trainings.Add(training);
        await ctx.SaveChangesAsync(ct);
        return training.Id;
    }
}
