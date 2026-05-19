using Market.Application.Modules.Catalog.Baskets.Queries.GetById;

namespace Market.Application.Modules.Catalog.Baskets.Queries.GetByUserId;

public sealed class GetBasketByUserIdQueryHandler(IAppDbContext ctx, ISender sender)
    : IRequestHandler<GetBasketByUserIdQuery, GetBasketByIdQueryDto>
{
    public async Task<GetBasketByIdQueryDto> Handle(GetBasketByUserIdQuery request, CancellationToken ct)
    {
        if (request.UserId <= 0) throw new ValidationException("UserId is required.");

        var basketId = await ctx.Baskets.AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(ct);

        if (basketId is null) throw new MarketNotFoundException($"Basket for user (ID={request.UserId}) nije pronađen.");

        return await sender.Send(new GetBasketByIdQuery { Id = basketId.Value }, ct);
    }
}
