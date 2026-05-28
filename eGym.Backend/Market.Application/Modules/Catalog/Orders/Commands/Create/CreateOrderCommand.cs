namespace Market.Application.Modules.Catalog.Orders.Commands.Create;

public sealed class CreateOrderCommand : IRequest<int>
{
    public List<CreateOrderCommandItem> Items { get; set; } = [];
}

public sealed class CreateOrderCommandItem
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}
