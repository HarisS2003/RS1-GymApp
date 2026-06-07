namespace Market.Application.Modules.Catalog.Users.Queries.ListWithMembership;

public sealed class ListUsersWithMembershipQuery : BasePagedQuery<ListUsersWithMembershipQueryDto>
{
    public string? Search { get; init; }
    public int? GymId { get; init; }
    public int? RoleId { get; init; }
}
