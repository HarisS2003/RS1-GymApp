namespace Market.Application.Modules.Catalog.MembershipPlans.Queries.List;

public sealed class ListMembershipPlansQuery : BasePagedQuery<ListMembershipPlansQueryDto>
{
    public string? Search { get; init; }
    public int? GymId { get; init; }
}
