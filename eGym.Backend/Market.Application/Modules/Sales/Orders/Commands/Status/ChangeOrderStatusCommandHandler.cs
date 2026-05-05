using Market.Application.Modules.Sales.Orders.Commands.Status;

namespace Market.Application.Modules.Sales.Orders.Commands.ChangeStatus;

public class ChangeOrderStatusCommandHandler(IAppDbContext db) : IRequestHandler<ChangeOrderStatusCommand>
{
    public async Task Handle(ChangeOrderStatusCommand request, CancellationToken ct)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"{nameof(OrderEntity)}, {request.Id}");

        order.Status = request.NewStatus;
        await db.SaveChangesAsync(ct);
    }
}
