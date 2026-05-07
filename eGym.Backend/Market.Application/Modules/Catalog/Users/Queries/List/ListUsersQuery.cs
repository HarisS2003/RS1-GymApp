namespace Market.Application.Modules.Catalog.Users.Queries.List;

public sealed class ListUsersQuery : BasePagedQuery<ListUsersQueryDto>
{
    public string? Search { get; init; }
    public int? GymId { get; init; }
    public int? RoleId { get; init; }
}
