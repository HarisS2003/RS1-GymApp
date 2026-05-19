namespace Market.Application.Modules.Catalog.Trainings.Commands.Create;

public sealed class CreateTrainingCommand : IRequest<int>
{
    public int TrainerId { get; set; }
    public TrainingType Type { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public int Capacity { get; set; }
}
