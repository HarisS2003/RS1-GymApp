namespace Market.Application.Modules.Catalog.Baskets.Commands.Delete;

public sealed class DeleteBasketCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteBasketCommand, Unit>
{
    public async Task<Unit> Handle(DeleteBasketCommand request, CancellationToken ct)
    {
        var basket = await ctx.Baskets
            .Include(x => x.BasketItems)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (basket is null) throw new MarketNotFoundException("Basket nije pronađen.");

        basket.IsDeleted = true;
        basket.ModifiedAtUtc = DateTime.UtcNow;

        foreach (var item in basket.BasketItems)
        {
            item.IsDeleted = true;
            item.ModifiedAtUtc = DateTime.UtcNow;
        }

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
