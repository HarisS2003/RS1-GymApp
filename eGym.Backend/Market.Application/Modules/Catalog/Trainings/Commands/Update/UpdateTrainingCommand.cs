namespace Market.Application.Modules.Catalog.Trainings.Commands.Update;

public sealed class UpdateTrainingCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public int TrainerId { get; set; }
    public TrainingType Type { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public int Capacity { get; set; }
}
