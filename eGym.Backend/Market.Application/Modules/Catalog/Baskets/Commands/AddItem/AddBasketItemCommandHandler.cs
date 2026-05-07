namespace Market.Application.Modules.Catalog.Baskets.Commands.AddItem;

public sealed class AddBasketItemCommandHandler(IAppDbContext ctx)
    : IRequestHandler<AddBasketItemCommand, int>
{
    public async Task<int> Handle(AddBasketItemCommand request, CancellationToken ct)
    {
        if (request.BasketId <= 0) throw new ValidationException("BasketId is required.");
        if (request.ProductId <= 0) throw new ValidationException("ProductId is required.");
        if (request.Quantity <= 0) throw new ValidationException("Quantity must be positive.");

        var basketExists = await ctx.Baskets.AnyAsync(x => x.Id == request.BasketId, ct);
        if (!basketExists) throw new MarketNotFoundException($"Basket (ID={request.BasketId}) nije pronađen.");

        var product = await ctx.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, ct);
        if (product is null) throw new ValidationException($"Invalid productId {request.ProductId}.");

        var existing = await ctx.BasketItems.FirstOrDefaultAsync(
            x => x.BasketId == request.BasketId && x.ProductId == request.ProductId,
            ct);

        var newTotalQty = request.Quantity + (existing?.Quantity ?? 0);
        if (newTotalQty > product.StockQuantity)
            throw new ValidationException("Quantity exceeds available stock.");

        if (existing is not null)
        {
            existing.Quantity = newTotalQty;
            await ctx.SaveChangesAsync(ct);
            return existing.Id;
        }

        var row = new BasketItemEntity
        {
            BasketId = request.BasketId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };
        ctx.BasketItems.Add(row);
        await ctx.SaveChangesAsync(ct);
        return row.Id;
    }
}
