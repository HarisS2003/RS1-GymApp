namespace Market.Application.Modules.Catalog.Reviews.Commands.Create;

public sealed class CreateReviewCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateReviewCommand, int>
{
    public async Task<int> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        if (request.UserId <= 0) throw new ValidationException("UserId is required.");
        if (request.TrainerId <= 0) throw new ValidationException("TrainerId is required.");
        if (request.Rating is < 1 or > 5) throw new ValidationException("Rating must be between 1 and 5.");

        if (!await ctx.Users.AnyAsync(x => x.Id == request.UserId, ct))
            throw new ValidationException("Invalid UserId.");

        if (!await ctx.Trainers.AnyAsync(x => x.Id == request.TrainerId, ct))
            throw new ValidationException("Invalid TrainerId.");

        var comment = string.IsNullOrWhiteSpace(request.Comment)
            ? null
            : request.Comment.Trim();

        var review = new ReviewEntity
        {
            UserId = request.UserId,
            TrainerId = request.TrainerId,
            Rating = request.Rating,
            Comment = comment
        };

        ctx.Reviews.Add(review);
        await ctx.SaveChangesAsync(ct);
        return review.Id;
    }
}
