namespace Market.Application.Modules.Catalog.Orders.Commands.Update;

public sealed class UpdateOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateOrderCommand, Unit>
{
    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken ct)
    {
        var order = await FindOrderAsync(request.Id, ct)
            ?? throw new MarketNotFoundException($"Order (ID={request.Id}) nije pronađen.");

        if (!string.Equals(order.Status, OrderStatuses.Draft, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Only draft orders can be updated.");

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var status = request.Status.Trim();
            if (!OrderStatuses.IsValid(status))
                throw new ValidationException($"Invalid status '{status}'.");

            order.Status = status;
        }

        if (request.Items is not null)
        {
            if (request.Items.Count == 0)
                throw new ValidationException("Order must contain at least one item.");

            var existingItems = await ctx.OrderItems.Where(x => x.OrderId == order.Id).ToListAsync(ct);
            ctx.OrderItems.RemoveRange(existingItems);

            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await ctx.Products
                .Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
                .ToDictionaryAsync(p => p.Id, ct);

            order.TotalAmount = 0m;
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
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = price
                });
                order.TotalAmount += price;
            }
        }

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }

    private async Task<OrderEntity?> FindOrderAsync(int id, CancellationToken ct)
    {
        var q = ctx.Orders.Where(x => x.Id == id && !x.IsDeleted);
        if (!currentUser.IsAdmin)
            q = q.Where(x => x.UserId == currentUser.UserId);

        return await q.FirstOrDefaultAsync(ct);
    }
}
