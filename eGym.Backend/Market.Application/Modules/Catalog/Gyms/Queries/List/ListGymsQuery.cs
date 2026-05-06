namespace Market.Application.Modules.Catalog.Gyms.Queries.List;

public sealed class ListGymsQuery : BasePagedQuery<ListGymsQueryDto>
{
    public string? Search { get; init; }
    public string? City { get; init; }
}
