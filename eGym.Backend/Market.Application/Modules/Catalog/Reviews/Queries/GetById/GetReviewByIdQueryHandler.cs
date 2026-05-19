namespace Market.Application.Modules.Catalog.Reviews.Queries.GetById;

public sealed class GetReviewByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetReviewByIdQuery, GetReviewByIdQueryDto>
{
    public async Task<GetReviewByIdQueryDto> Handle(GetReviewByIdQuery request, CancellationToken ct)
    {
        var dto = await ctx.Reviews.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetReviewByIdQueryDto
            {
                Id = x.Id,
                UserId = x.UserId,
                TrainerId = x.TrainerId,
                Rating = x.Rating,
                Comment = x.Comment
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Review (ID={request.Id}) nije pronađen.");
    }
}
