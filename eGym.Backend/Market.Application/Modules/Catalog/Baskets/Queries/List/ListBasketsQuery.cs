namespace Market.Application.Modules.Catalog.Baskets.Queries.List;

public sealed class ListBasketsQuery : BasePagedQuery<ListBasketsQueryDto>
{
    public int? UserId { get; init; }
}
