namespace Market.Application.Modules.Catalog.Gyms.Commands.Delete;

public class DeleteGymCommandHandler(IAppDbContext ctx)
      : IRequestHandler<DeleteGymCommand, Unit>
{
    public async Task<Unit> Handle(DeleteGymCommand request, CancellationToken ct)
    {
        var gym = await ctx.Gyms.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
        if (gym is null) throw new MarketNotFoundException("Gym nije pronađen.");

        gym.IsDeleted = true;
        gym.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
