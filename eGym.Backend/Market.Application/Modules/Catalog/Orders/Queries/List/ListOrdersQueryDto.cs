namespace Market.Application.Modules.Catalog.Orders.Queries.List;

public sealed class ListOrdersQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required string Status { get; init; }
    public required decimal TotalAmount { get; init; }
}
