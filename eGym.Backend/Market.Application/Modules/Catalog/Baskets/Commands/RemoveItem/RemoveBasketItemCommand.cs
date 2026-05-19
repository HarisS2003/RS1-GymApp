namespace Market.Application.Modules.Catalog.Baskets.Commands.RemoveItem;

public sealed class RemoveBasketItemCommand : IRequest<Unit>
{
    public required int BasketId { get; set; }
    public required int ItemId { get; set; }
}
