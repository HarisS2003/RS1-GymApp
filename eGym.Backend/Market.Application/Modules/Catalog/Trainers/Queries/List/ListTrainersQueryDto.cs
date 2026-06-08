namespace Market.Application.Modules.Catalog.Trainers.Queries.List;

public sealed class ListTrainersQueryDto
{
    public required string PublicId { get; init; }
    public required string UserPublicId { get; init; }
    public required int GymId { get; init; }
    public required string Bio { get; init; }
    public required int ExperienceYears { get; init; }
}
