namespace eGymSystem.Application.Modules.Auth.Commands.Login;

public sealed class LoginCommandDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
