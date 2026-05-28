using Market.Application.Modules.Catalog.Products;

namespace Market.Application.Modules.Catalog.Products.Queries.GetById;

public class GetProductByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string CategoryName { get; init; }
    public string? Description { get; init; }
    public required decimal Price { get; set; }
    public required int StockQuantity { get; set; }
    public required int GymId { get; set; }
    public bool IsEnabled { get; init; } = true;
    public IReadOnlyList<ProductVariantQueryDto> ProductVariants { get; init; } = [];
}
