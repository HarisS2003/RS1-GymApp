namespace Market.Application.Modules.Identity.UserMemberships.Queries.ListMyHistory;

public sealed class ListMyMembershipPurchaseHistoryQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ListMyMembershipPurchaseHistoryQuery, List<ListMyMembershipPurchaseHistoryQueryDto>>
{
    public async Task<List<ListMyMembershipPurchaseHistoryQueryDto>> Handle(
        ListMyMembershipPurchaseHistoryQuery request,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return [];

        var today = DateTime.UtcNow.Date;

        var rows = await (
            from m in ctx.UserMemberships.AsNoTracking()
            join p in ctx.MembershipPlans.AsNoTracking() on m.MembershipPlanId equals p.Id
            where m.UserId == userId
            orderby m.StartDate descending
            select new { m, p }
        ).ToListAsync(ct);

        return rows.Select(row =>
        {
            var discount = row.p.Price * row.p.DiscountPercentage / 100m;
            var finalPrice = Math.Round(row.p.Price - discount, 2, MidpointRounding.AwayFromZero);

            return new ListMyMembershipPurchaseHistoryQueryDto
            {
                UserMembershipId = row.m.Id,
                PlanName = row.p.Name,
                AmountPaid = finalPrice,
                PurchasedAt = row.m.StartDate,
                EndDate = row.m.EndDate,
                DurationDays = row.p.DurationDays,
                IsActive = row.m.EndDate.Date >= today && row.m.StartDate.Date <= today,
            };
        }).ToList();
    }
}
