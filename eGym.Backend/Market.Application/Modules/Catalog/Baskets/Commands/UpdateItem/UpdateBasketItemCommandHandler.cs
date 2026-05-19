namespace Market.Application.Modules.Catalog.Baskets.Commands.UpdateItem;

public sealed class UpdateBasketItemCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateBasketItemCommand, Unit>
{
    public async Task<Unit> Handle(UpdateBasketItemCommand request, CancellationToken ct)
    {
        if (request.BasketId <= 0) throw new ValidationException("BasketId is required.");
        if (request.ItemId <= 0) throw new ValidationException("ItemId is required.");
        if (request.Quantity <= 0) throw new ValidationException("Quantity must be positive.");

        var item = await ctx.BasketItems.FirstOrDefaultAsync(
            x => x.Id == request.ItemId && x.BasketId == request.BasketId,
            ct);
        if (item is null) throw new MarketNotFoundException("Basket item nije pronađen.");

        var product = await ctx.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId, ct);
        if (product is null) throw new ValidationException($"Invalid productId {item.ProductId}.");

        if (request.Quantity > product.StockQuantity)
            throw new ValidationException("Quantity exceeds available stock.");

        item.Quantity = request.Quantity;
        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
