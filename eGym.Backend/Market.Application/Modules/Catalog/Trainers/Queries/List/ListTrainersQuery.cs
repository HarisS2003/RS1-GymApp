namespace Market.Application.Modules.Catalog.Trainers.Queries.List;

public sealed class ListTrainersQuery : BasePagedQuery<ListTrainersQueryDto>
{
    public string? Search { get; init; }
    public int? GymId { get; init; }
    public int? UserId { get; init; }
    public int? MinExperienceYears { get; init; }
}
