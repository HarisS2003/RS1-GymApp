namespace Market.Application.Modules.Catalog.Reviews.Queries.List;

public sealed class ListReviewsQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required int TrainerId { get; init; }
    public required int Rating { get; init; }
    public string? Comment { get; init; }
}
