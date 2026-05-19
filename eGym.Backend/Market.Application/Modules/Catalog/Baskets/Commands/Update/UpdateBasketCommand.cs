namespace Market.Application.Modules.Catalog.Baskets.Commands.Update;

public sealed class UpdateBasketCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public int UserId { get; set; }
}
