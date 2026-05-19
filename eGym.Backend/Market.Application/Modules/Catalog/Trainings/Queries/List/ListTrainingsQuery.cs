namespace Market.Application.Modules.Catalog.Trainings.Queries.List;

public sealed class ListTrainingsQuery : BasePagedQuery<ListTrainingsQueryDto>
{
    public int? TrainerId { get; init; }
    public TrainingType? Type { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}
