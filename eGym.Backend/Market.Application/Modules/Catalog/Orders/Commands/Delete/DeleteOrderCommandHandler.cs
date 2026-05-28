namespace Market.Application.Modules.Catalog.Orders.Commands.Delete;

public sealed class DeleteOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteOrderCommand, Unit>
{
    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken ct)
    {
        var q = ctx.Orders
            .Include(x => x.OrderItems)
            .Where(x => x.Id == request.Id && !x.IsDeleted);

        if (!currentUser.IsAdmin)
            q = q.Where(x => x.UserId == currentUser.UserId);

        var order = await q.FirstOrDefaultAsync(ct);
        if (order is null) throw new MarketNotFoundException($"Order (ID={request.Id}) nije pronađen.");

        if (!string.Equals(order.Status, OrderStatuses.Draft, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Only draft orders can be deleted.");

        var now = DateTime.UtcNow;
        order.IsDeleted = true;
        order.ModifiedAtUtc = now;

        foreach (var item in order.OrderItems)
        {
            item.IsDeleted = true;
            item.ModifiedAtUtc = now;
        }

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
