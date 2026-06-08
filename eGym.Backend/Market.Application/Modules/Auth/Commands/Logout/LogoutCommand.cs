namespace Market.Application.Modules.Auth.Commands.Logout;

public sealed class LogoutCommand : IRequest<Unit>
{
    public string RefreshToken { get; init; } = string.Empty;
}
