namespace Market.Application.Modules.Catalog.Users.Commands.Delete;

public class DeleteUserCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<DeleteUserCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var user = await ctx.Users.FirstOrDefaultAsync(
            x => x.PublicId == request.PublicId && x.GymId == currentGymId,
            ct);
        if (user is null) throw new MarketNotFoundException("User not found.");

        user.IsDeleted = true;
        user.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }

    private async Task<int> GetCurrentGymIdAsync(CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var gymId = await ctx.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.GymId)
            .FirstOrDefaultAsync(ct);

        return gymId == 0
            ? throw new MarketNotFoundException("User not found.")
            : gymId;
    }
}
