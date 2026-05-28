namespace Market.Application.Modules.Catalog.Orders.Queries.List;

public sealed class ListOrdersQuery : BasePagedQuery<ListOrdersQueryDto>
{
    public string? Search { get; init; }
    public string? Status { get; init; }
}
