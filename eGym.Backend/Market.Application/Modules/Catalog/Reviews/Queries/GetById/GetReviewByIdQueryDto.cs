namespace Market.Application.Modules.Catalog.Reviews.Queries.GetById;

public sealed class GetReviewByIdQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required int TrainerId { get; init; }
    public required int Rating { get; init; }
    public string? Comment { get; init; }
}
