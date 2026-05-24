namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.ListForTrainer;

public sealed class ListTrainerTrainingRequestsQuery : IRequest<List<ListTrainerTrainingRequestQueryDto>>
{
    public TrainingRequestStatus? Status { get; set; }
}
