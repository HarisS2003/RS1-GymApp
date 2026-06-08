namespace Market.Application.Modules.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IAppDbContext ctx,
    IJwtTokenService jwt,
    TimeProvider time)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandDto>
{
    public async Task<RefreshTokenCommandDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var hash = jwt.HashRefreshToken(request.RefreshToken.Trim());
        var now = time.GetUtcNow().UtcDateTime;

        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.RefreshTokenHash == hash && !x.IsDeleted, ct)
            ?? throw new MarketConflictException("Refresh token is invalid.");

        if (user.RefreshTokenExpiresAtUtc is null || user.RefreshTokenExpiresAtUtc <= now)
            throw new MarketConflictException("Refresh token expired.");

        var tokens = jwt.IssueTokens(user);
        user.RefreshTokenHash = tokens.RefreshTokenHash;
        user.RefreshTokenExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc;

        await ctx.SaveChangesAsync(ct);

        return new RefreshTokenCommandDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshTokenRaw,
            AccessTokenExpiresAtUtc = tokens.AccessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc
        };
    }
}
