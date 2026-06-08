namespace Market.Application.Modules.Catalog.Orders.Commands.Create;

public sealed class CreateOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new ValidationException("Current user is required.");
        if (request.Items.Count == 0) throw new ValidationException("Order must contain at least one item.");

        var order = new OrderEntity
        {
            UserId = userId,
            Status = OrderStatuses.Draft,
            TotalAmount = 0m
        };
        ctx.Orders.Add(order);

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await ctx.Products
            .Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
            .ToDictionaryAsync(p => p.Id, ct);

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0) throw new ValidationException("Quantity must be greater than zero.");
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new ValidationException($"Invalid productId {item.ProductId}.");

            if (item.Quantity > product.StockQuantity)
                throw new ValidationException($"Insufficient stock for product '{product.Name}'.");

            var price = Math.Round(product.Price * item.Quantity, 2, MidpointRounding.AwayFromZero);
            ctx.OrderItems.Add(new OrderItemEntity
            {
                Order = order,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = price
            });
            order.TotalAmount += price;
        }

        await ctx.SaveChangesAsync(ct);
        return order.Id;
    }
}
