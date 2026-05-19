namespace Market.Application.Modules.Catalog.Baskets.Commands.Create;

public sealed class CreateBasketCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateBasketCommand, int>
{
    public async Task<int> Handle(CreateBasketCommand request, CancellationToken ct)
    {
        if (request.UserId <= 0) throw new ValidationException("UserId is required.");

        if (!await ctx.Users.AnyAsync(x => x.Id == request.UserId, ct))
            throw new ValidationException("Invalid UserId.");

        var exists = await ctx.Baskets.AnyAsync(x => x.UserId == request.UserId, ct);
        if (exists) throw new MarketConflictException("Basket already exists for this user.");

        var basket = new BasketEntity { UserId = request.UserId };
        ctx.Baskets.Add(basket);
        await ctx.SaveChangesAsync(ct);
        return basket.Id;
    }
}
