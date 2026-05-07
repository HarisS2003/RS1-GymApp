namespace Market.Application.Modules.Catalog.Reviews.Commands.Update;

public sealed class UpdateReviewCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateReviewCommand, Unit>
{
    public async Task<Unit> Handle(UpdateReviewCommand request, CancellationToken ct)
    {
        var entity = await ctx.Reviews.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) throw new MarketNotFoundException($"Review (ID={request.Id}) nije pronađen.");

        if (request.Rating is < 1 or > 5) throw new ValidationException("Rating must be between 1 and 5.");

        var comment = string.IsNullOrWhiteSpace(request.Comment)
            ? null
            : request.Comment.Trim();

        entity.Rating = request.Rating;
        entity.Comment = comment;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
