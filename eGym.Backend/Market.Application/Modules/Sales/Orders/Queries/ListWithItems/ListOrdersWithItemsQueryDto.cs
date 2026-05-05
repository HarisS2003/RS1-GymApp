namespace Market.Application.Modules.Sales.Orders.Queries.ListWithItems;

public sealed class ListOrdersWithItemsQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required string Status { get; set; }
    public required decimal TotalAmount { get; set; }
    public required List<ListOrdersWithItemsQueryDtoItem> Items { get; set; }
}

public class ListOrdersWithItemsQueryDtoItem
{
    public required int Id { get; set; }
    public required int ProductId { get; set; }
    public required decimal Quantity { get; set; }
    public required decimal Price { get; set; }
}
