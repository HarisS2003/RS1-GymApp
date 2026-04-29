namespace eGymSystem.Application.Modules.Catalog.Products.Queries.List;

public sealed class ListProductsQueryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
