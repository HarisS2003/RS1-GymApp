namespace Market.Application.Modules.Identity.UserMemberships.Queries.GetMyActive;

public sealed class GetMyActiveUserMembershipQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyActiveUserMembershipQuery, GetMyActiveUserMembershipQueryDto?>
{
    public async Task<GetMyActiveUserMembershipQueryDto?> Handle(
        GetMyActiveUserMembershipQuery request,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return null;

        var today = DateTime.UtcNow.Date;

        var row = await (
            from m in ctx.UserMemberships.AsNoTracking()
            join p in ctx.MembershipPlans.AsNoTracking() on m.MembershipPlanId equals p.Id
            where m.UserId == userId && m.EndDate >= today
            orderby m.EndDate descending
            select new { m, p }
        ).FirstOrDefaultAsync(ct);

        if (row is null)
            return null;

        var discount = row.p.Price * row.p.DiscountPercentage / 100m;
        var finalPrice = Math.Round(row.p.Price - discount, 2, MidpointRounding.AwayFromZero);

        return new GetMyActiveUserMembershipQueryDto
        {
            UserMembershipId = row.m.Id,
            MembershipPlanId = row.p.Id,
            PlanName = row.p.Name,
            DurationDays = row.p.DurationDays,
            Price = row.p.Price,
            DiscountPercentage = row.p.DiscountPercentage,
            FinalPrice = finalPrice,
            StartDate = row.m.StartDate,
            EndDate = row.m.EndDate,
        };
    }
}
