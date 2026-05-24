namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Create;

public sealed class CreateTrainingRequestCommand : IRequest<CreateTrainingRequestResultDto>
{
    public int TrainerId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
}
