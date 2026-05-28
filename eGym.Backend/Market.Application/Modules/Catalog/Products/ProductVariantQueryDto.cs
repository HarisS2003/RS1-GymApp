namespace Market.Application.Modules.Catalog.Products;

public sealed class ProductVariantQueryDto
{
    public required int Id { get; init; }
    public required int ProductId { get; init; }
    public required string Size { get; init; }
    public required string Color { get; init; }
    public required decimal Price { get; init; }
    public required int StockQuantity { get; init; }
}
