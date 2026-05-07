namespace Market.Application.Modules.Catalog.Users.Queries.List;

public sealed class ListUsersQueryDto
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required int RoleId { get; init; }
    public required int GymId { get; init; }
}
