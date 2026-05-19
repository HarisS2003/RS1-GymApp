namespace Market.Application.Modules.Catalog.Reviews.Commands.Delete;

public sealed class DeleteReviewCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteReviewCommand, Unit>
{
    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        var review = await ctx.Reviews.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (review is null) throw new MarketNotFoundException("Review nije pronađen.");

        review.IsDeleted = true;
        review.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
