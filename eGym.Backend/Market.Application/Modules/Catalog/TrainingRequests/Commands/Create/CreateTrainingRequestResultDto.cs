namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Create;

public sealed class CreateTrainingRequestResultDto
{
    public int TrainingRequestId { get; set; }
    public int TrainerId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TrainingRequestStatus Status { get; set; }
}
