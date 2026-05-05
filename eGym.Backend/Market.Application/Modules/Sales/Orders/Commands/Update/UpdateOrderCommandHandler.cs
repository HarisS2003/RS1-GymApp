namespace Market.Application.Modules.Sales.Orders.Commands.Update;

public class UpdateOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateOrderCommand, int>
{
    public async Task<int> Handle(UpdateOrderCommand request, CancellationToken ct)
    {
        var q = ctx.Orders.Where(x => x.Id == request.Id);
        if (!currentUser.IsAdmin)
        {
            q = q.Where(x => x.UserId == currentUser.UserId);
        }

        var order = await q.FirstOrDefaultAsync(ct);
        if (order is null) throw new MarketNotFoundException($"Orders (ID={request.Id}) nije pronađen.");

        if (!string.Equals(order.Status, "Draft", StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Only draft orders can be updated.");

        var existingItems = await ctx.OrderItems.Where(oi => oi.OrderId == order.Id).ToListAsync(ct);
        var existingMap = existingItems.ToDictionary(x => x.Id);

        var itemsToDelete = existingItems.Where(oi => request.Items.All(ri => ri.Id != oi.Id)).ToList();
        ctx.OrderItems.RemoveRange(itemsToDelete);

        var productIds = request.Items.Select(ri => ri.ProductId).Distinct().ToList();
        var products = await ctx.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(x => x.Id, ct);

        order.TotalAmount = 0m;

        foreach (var item in request.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new ValidationException($"Invalid productId {item.ProductId}.");

            var price = Math.Round(product.Price * item.Quantity, 2, MidpointRounding.AwayFromZero);

            if (item.Id == 0)
            {
                var newItem = new OrderItemEntity
                {
                    Order = order,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = price
                };
                ctx.OrderItems.Add(newItem);
                order.TotalAmount += price;
            }
            else
            {
                var existing = existingMap.GetValueOrDefault(item.Id)
                    ?? throw new ValidationException($"Order item (ID={item.Id}) not found in order (ID={order.Id}).");

                existing.ProductId = item.ProductId;
                existing.Quantity = item.Quantity;
                existing.Price = price;
                order.TotalAmount += price;
            }
        }

        await ctx.SaveChangesAsync(ct);
        return order.Id;
    }
}
