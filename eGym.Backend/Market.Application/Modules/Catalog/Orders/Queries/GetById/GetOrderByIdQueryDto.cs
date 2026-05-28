namespace Market.Application.Modules.Catalog.Orders.Queries.GetById;

public sealed class GetOrderByIdQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required string Status { get; init; }
    public required decimal TotalAmount { get; init; }
    public required List<GetOrderByIdQueryDtoItem> Items { get; init; }
}

public sealed class GetOrderByIdQueryDtoItem
{
    public required int Id { get; init; }
    public required int ProductId { get; init; }
    public required string ProductName { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal Price { get; init; }
}
