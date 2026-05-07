namespace Market.Application.Modules.Catalog.Baskets.Commands.Delete;

public sealed class DeleteBasketCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
