using Market.Shared.Constants;

namespace Market.Application.Modules.Catalog.Trainers.Commands.Delete;

public sealed class DeleteTrainerCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<DeleteTrainerCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTrainerCommand request, CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var trainer = await ctx.Trainers.FirstOrDefaultAsync(
            x => x.PublicId == request.PublicId && x.GymId == currentGymId && !x.IsDeleted,
            ct);
        if (trainer is null) throw new MarketNotFoundException("Trainer not found.");

        trainer.IsDeleted = true;
        trainer.ModifiedAtUtc = DateTime.UtcNow;

        var user = await ctx.Users.FirstOrDefaultAsync(x => x.Id == trainer.UserId && !x.IsDeleted, ct);
        if (user is not null && user.RoleId == RoleIds.Trainer)
        {
            user.RoleId = RoleIds.Member;
            user.ModifiedAtUtc = DateTime.UtcNow;
        }

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
