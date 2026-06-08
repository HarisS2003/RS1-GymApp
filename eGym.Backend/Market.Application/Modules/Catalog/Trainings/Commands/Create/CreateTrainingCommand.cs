namespace Market.Application.Modules.Catalog.Trainings.Commands.Create;

public sealed class CreateTrainingCommand : IRequest<int>
{
    public string TrainerPublicId { get; set; } = string.Empty;
    public TrainingType Type { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public int Capacity { get; set; }
}
