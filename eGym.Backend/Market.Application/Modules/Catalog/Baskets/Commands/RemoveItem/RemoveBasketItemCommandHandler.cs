namespace Market.Application.Modules.Catalog.Baskets.Commands.RemoveItem;

public sealed class RemoveBasketItemCommandHandler(IAppDbContext ctx)
    : IRequestHandler<RemoveBasketItemCommand, Unit>
{
    public async Task<Unit> Handle(RemoveBasketItemCommand request, CancellationToken ct)
    {
        var item = await ctx.BasketItems.FirstOrDefaultAsync(
            x => x.Id == request.ItemId && x.BasketId == request.BasketId,
            ct);
        if (item is null) throw new MarketNotFoundException("Basket item nije pronađen.");

        item.IsDeleted = true;
        item.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
