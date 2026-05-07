namespace Market.Application.Modules.Catalog.Reviews.Queries.List;

public sealed class ListReviewsQuery : BasePagedQuery<ListReviewsQueryDto>
{
    public int? TrainerId { get; init; }
    public int? UserId { get; init; }
    public int? MinRating { get; init; }
    public int? MaxRating { get; init; }
    public string? Search { get; init; }
}
