namespace Market.Application.Modules.Catalog.Products;

public sealed class ProductVariantCommandDto
{
    public int? Id { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
