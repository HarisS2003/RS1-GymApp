namespace Market.Application.Modules.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommand : IRequest<RefreshTokenCommandDto>
{
    public string RefreshToken { get; init; } = string.Empty;
    public string? Fingerprint { get; init; }
}
