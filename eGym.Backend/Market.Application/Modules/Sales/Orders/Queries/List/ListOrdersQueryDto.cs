namespace Market.Application.Modules.Sales.Orders.Queries.List;

public sealed class ListOrdersQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required string Status { get; set; }
    public required decimal TotalAmount { get; set; }
}
