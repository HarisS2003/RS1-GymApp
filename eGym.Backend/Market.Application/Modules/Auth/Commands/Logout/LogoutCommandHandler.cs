namespace Market.Application.Modules.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(
    IAppDbContext ctx,
    IJwtTokenService jwt)
    : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken ct)
    {
        var hash = jwt.HashRefreshToken(request.RefreshToken.Trim());

        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.RefreshTokenHash == hash && !x.IsDeleted, ct);

        if (user is not null)
        {
            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresAtUtc = null;
            await ctx.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}
