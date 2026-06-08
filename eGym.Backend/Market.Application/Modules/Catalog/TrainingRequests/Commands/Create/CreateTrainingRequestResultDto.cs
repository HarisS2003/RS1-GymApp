namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Create;

public sealed class CreateTrainingRequestResultDto
{
    public int TrainingRequestId { get; set; }
    public string TrainerPublicId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TrainingRequestStatus Status { get; set; }
}
