namespace Market.Application.Modules.Catalog.Orders.Commands.Update;

public sealed class UpdateOrderCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string? Status { get; set; }
    public List<UpdateOrderCommandItem>? Items { get; set; }
}

public sealed class UpdateOrderCommandItem
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}
