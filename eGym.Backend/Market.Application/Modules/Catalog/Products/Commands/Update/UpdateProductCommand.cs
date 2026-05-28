using Market.Application.Modules.Catalog.Products;

namespace Market.Application.Modules.Catalog.Products.Commands.Update;

public sealed class UpdateProductCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int GymId { get; set; }
    public List<ProductVariantCommandDto> Variants { get; set; } = new();
}
