namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Approve;

public sealed class ApproveTrainingRequestResultDto
{
    public int TrainingRequestId { get; set; }
    public int TrainingId { get; set; }
    public TrainingRequestStatus Status { get; set; }
}
