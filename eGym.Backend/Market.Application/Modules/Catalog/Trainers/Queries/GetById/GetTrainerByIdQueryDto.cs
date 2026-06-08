namespace Market.Application.Modules.Catalog.Trainers.Queries.GetById;

public sealed class GetTrainerByIdQueryDto
{
    public required string PublicId { get; init; }
    public required string UserPublicId { get; init; }
    public required int GymId { get; init; }
    public required string Bio { get; init; }
    public required int ExperienceYears { get; init; }
}
