namespace Market.Application.Modules.Catalog.Products.Queries.List;

public sealed class ListProductsQuery : BasePagedQuery<ListProductsQueryDto>
{
    public string? Search { get; init; }
    public int? GymId { get; init; }
    public string? Size { get; init; }
    public string? CategoryName { get; init; }
}
