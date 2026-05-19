namespace Market.Application.Modules.Catalog.Baskets.Commands.UpdateItem;

public sealed class UpdateBasketItemCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int BasketId { get; set; }

    [JsonIgnore]
    public int ItemId { get; set; }

    public int Quantity { get; set; }
}
