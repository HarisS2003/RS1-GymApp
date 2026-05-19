namespace Market.Application.Modules.Catalog.Reviews.Queries.List;

public sealed class ListReviewsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListReviewsQuery, PageResult<ListReviewsQueryDto>>
{
    public async Task<PageResult<ListReviewsQueryDto>> Handle(ListReviewsQuery request, CancellationToken ct)
    {
        var q = ctx.Reviews.AsNoTracking();

        if (request.TrainerId is int trainerId)
            q = q.Where(x => x.TrainerId == trainerId);

        if (request.UserId is int userId)
            q = q.Where(x => x.UserId == userId);

        if (request.MinRating is int minRating)
            q = q.Where(x => x.Rating >= minRating);

        if (request.MaxRating is int maxRating)
            q = q.Where(x => x.Rating <= maxRating);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            q = q.Where(x => x.Comment != null && x.Comment.ToLower().Contains(term));
        }

        var projectedQuery = q.Select(x => new ListReviewsQueryDto
        {
            Id = x.Id,
            UserId = x.UserId,
            TrainerId = x.TrainerId,
            Rating = x.Rating,
            Comment = x.Comment
        });

        return await PageResult<ListReviewsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
