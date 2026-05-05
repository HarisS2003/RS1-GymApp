namespace Market.Application.Modules.Sales.Orders.Commands.Create;

public class CreateOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new ValidationException("Current user is required.");

        var order = new OrderEntity
        {
            UserId = userId,
            Status = "Draft",
            TotalAmount = 0m
        };
        ctx.Orders.Add(order);

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await ctx.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(x => x.Id, ct);

        foreach (var item in request.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new ValidationException($"Invalid productId {item.ProductId}.");

            var price = Math.Round(product.Price * item.Quantity, 2, MidpointRounding.AwayFromZero);
            var orderItem = new OrderItemEntity
            {
                Order = order,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = price
            };

            ctx.OrderItems.Add(orderItem);
            order.TotalAmount += price;
        }

        await ctx.SaveChangesAsync(ct);
        return order.Id;
    }
}

