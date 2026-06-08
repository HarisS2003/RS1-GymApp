namespace Market.Application.Modules.Catalog.Users.Commands.Update;

public sealed class UpdateUserCommand : IRequest<Unit>
{
    public string PublicId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Password { get; set; }
    public int RoleId { get; set; }
    public int GymId { get; set; }
}
