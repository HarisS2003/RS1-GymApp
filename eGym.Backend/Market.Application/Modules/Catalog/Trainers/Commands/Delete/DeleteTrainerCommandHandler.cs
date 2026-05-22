using Market.Shared.Constants;

namespace Market.Application.Modules.Catalog.Trainers.Commands.Delete;

public sealed class DeleteTrainerCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteTrainerCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTrainerCommand request, CancellationToken ct)
    {
        var trainer = await ctx.Trainers.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
        if (trainer is null) throw new MarketNotFoundException("Trainer nije pronađen.");

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
}