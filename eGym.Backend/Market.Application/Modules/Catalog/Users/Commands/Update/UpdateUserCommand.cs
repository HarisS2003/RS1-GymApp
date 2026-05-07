namespace Market.Application.Modules.Catalog.Users.Commands.Update;

public sealed class UpdateUserCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public int RoleId { get; set; }
    public int GymId { get; set; }
}
