namespace Market.Application.Modules.Catalog.Baskets.Queries.List;

public sealed class ListBasketsQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required int ItemsCount { get; init; }
}
