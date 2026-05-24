namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Approve;

public sealed class ApproveTrainingRequestCommand : IRequest<ApproveTrainingRequestResultDto>
{
    public int Id { get; set; }
}
