using Market.Domain.Common;

namespace Market.Domain.Entities;

public class ProductVariantEntity : BaseEntity
{
    public int ProductId { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public ProductEntity? Product { get; set; }
}
