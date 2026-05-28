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

        // Left join — membership must not disappear when plan row missing/deleted
        var row = await (
            from m in ctx.UserMemberships.AsNoTracking()
            join p in ctx.MembershipPlans.AsNoTracking() on m.MembershipPlanId equals p.Id into plans
            from p in plans.DefaultIfEmpty()
            where m.UserId == userId && m.EndDate.Date >= today
            orderby m.EndDate descending
            select new { m, p }
        ).FirstOrDefaultAsync(ct);

        if (row is null)
            return null;

        var price = row.p?.Price ?? 0m;
        var discountPct = row.p?.DiscountPercentage ?? 0m;
        var discount = price * discountPct / 100m;
        var finalPrice = Math.Round(price - discount, 2, MidpointRounding.AwayFromZero);

        return new GetMyActiveUserMembershipQueryDto
        {
            UserMembershipId = row.m.Id,
            MembershipPlanId = row.m.MembershipPlanId,
            PlanName = row.p?.Name ?? "Membership",
            DurationDays = row.p?.DurationDays ?? Math.Max(1, (row.m.EndDate.Date - row.m.StartDate.Date).Days),
            Price = price,
            DiscountPercentage = discountPct,
            FinalPrice = finalPrice,
            StartDate = row.m.StartDate,
            EndDate = row.m.EndDate,
        };
    }
}
