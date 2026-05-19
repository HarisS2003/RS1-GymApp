namespace Market.Application.Modules.Catalog.Baskets.Commands.Update;

public sealed class UpdateBasketCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateBasketCommand, Unit>
{
    public async Task<Unit> Handle(UpdateBasketCommand request, CancellationToken ct)
    {
        var entity = await ctx.Baskets.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) throw new MarketNotFoundException($"Basket (ID={request.Id}) nije pronađen.");

        if (request.UserId <= 0) throw new ValidationException("UserId is required.");

        if (!await ctx.Users.AnyAsync(x => x.Id == request.UserId, ct))
            throw new ValidationException("Invalid UserId.");

        var duplicate = await ctx.Baskets.AnyAsync(
            x => x.Id != request.Id && x.UserId == request.UserId,
            ct);
        if (duplicate) throw new MarketConflictException("Basket already exists for this user.");

        entity.UserId = request.UserId;
        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
