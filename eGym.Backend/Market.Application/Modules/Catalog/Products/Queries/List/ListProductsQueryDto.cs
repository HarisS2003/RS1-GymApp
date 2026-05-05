namespace Market.Application.Modules.Catalog.Products.Queries.List;

public sealed class ListProductsQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; set; }
    public required int StockQuantity { get; set; }
    public required int GymId { get; set; }
}
