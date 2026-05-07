namespace Market.Application.Modules.Catalog.MembershipPlans.Queries.GetById;

public sealed class GetMembershipPlanByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetMembershipPlanByIdQuery, GetMembershipPlanByIdQueryDto>
{
    public async Task<GetMembershipPlanByIdQueryDto> Handle(GetMembershipPlanByIdQuery request, CancellationToken ct)
    {
        var dto = await ctx.MembershipPlans.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetMembershipPlanByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                DurationDays = x.DurationDays,
                Price = x.Price,
                DiscountPercentage = x.DiscountPercentage,
                GymId = x.GymId
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Membership plan (ID={request.Id}) nije pronađen.");
    }
}
