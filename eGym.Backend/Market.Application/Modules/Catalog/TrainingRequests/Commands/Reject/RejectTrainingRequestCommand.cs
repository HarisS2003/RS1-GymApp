namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Reject;

public sealed class RejectTrainingRequestCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
