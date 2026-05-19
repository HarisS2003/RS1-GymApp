namespace Market.Application.Modules.Catalog.Baskets.Queries.GetById;

public sealed class BasketItemRowDto
{
    public required int Id { get; init; }
    public required int ProductId { get; init; }
    public required string ProductName { get; init; }
    public required decimal UnitPrice { get; init; }
    public required int Quantity { get; init; }
    public required decimal LineTotal { get; init; }
}

public sealed class GetBasketByIdQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required IReadOnlyList<BasketItemRowDto> Items { get; init; }
}
