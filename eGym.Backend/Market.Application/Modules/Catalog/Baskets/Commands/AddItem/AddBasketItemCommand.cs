namespace Market.Application.Modules.Catalog.Baskets.Commands.AddItem;

public sealed class AddBasketItemCommand : IRequest<int>
{
    [JsonIgnore]
    public int BasketId { get; set; }

    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
