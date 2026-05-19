namespace Market.Application.Modules.Catalog.Trainings.Queries.GetById;

public sealed class GetTrainingByIdQueryDto
{
    public required int Id { get; init; }
    public required int TrainerId { get; init; }
    public required TrainingType Type { get; init; }
    public required DateTime Date { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required int Capacity { get; init; }
    public required int ParticipantsCount { get; init; }
}
