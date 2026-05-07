namespace Market.Application.Modules.Catalog.Users.Commands.Delete;

public class DeleteUserCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteUserCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (user is null) throw new MarketNotFoundException("User nije pronađen.");

        user.IsDeleted = true;
        user.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
