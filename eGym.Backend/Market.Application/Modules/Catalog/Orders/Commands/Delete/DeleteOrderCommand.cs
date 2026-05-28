namespace Market.Application.Modules.Catalog.Orders.Commands.Delete;

public sealed class DeleteOrderCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
