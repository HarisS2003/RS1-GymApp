namespace Market.Application.Modules.Catalog.Users.Queries.ListWithMembership;

public sealed class ListUsersWithMembershipQueryDto
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public int? UserMembershipId { get; init; }
    public string? CurrentMembershipName { get; init; }
    public required string MembershipStatus { get; init; }
}
