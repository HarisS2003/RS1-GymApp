namespace Market.Application.Modules.Catalog.Baskets.Commands.Create;

public sealed class CreateBasketCommand : IRequest<int>
{
    public int UserId { get; set; }
}
