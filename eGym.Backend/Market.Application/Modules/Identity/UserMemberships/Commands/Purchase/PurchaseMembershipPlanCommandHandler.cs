namespace Market.Application.Modules.Identity.UserMemberships.Commands.Purchase;

public sealed class PurchaseMembershipPlanCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<PurchaseMembershipPlanCommand, PurchaseMembershipPlanResultDto>
{
    public async Task<PurchaseMembershipPlanResultDto> Handle(
        PurchaseMembershipPlanCommand request,
        CancellationToken ct)
    {
        if (request.PaymentMethod != PaymentMethod.Cash)
            throw new ValidationException("Only cash payment is supported.");

        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var user = await ctx.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, ct)
            ?? throw new MarketNotFoundException("User not found.");

        var plan = await ctx.MembershipPlans.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.MembershipPlanId, ct)
            ?? throw new MarketNotFoundException("Membership plan not found.");

        if (user.GymId != plan.GymId)
            throw new ValidationException("Plan does not belong to your gym.");

        var amount = CalculateFinalPrice(plan.Price, plan.DiscountPercentage);
        if (amount <= 0)
            throw new ValidationException("Invalid plan price.");

        var now = DateTime.UtcNow;
        var startDate = now.Date;
        var endDate = startDate.AddDays(plan.DurationDays);

        var activeMemberships = await ctx.UserMemberships
            .Where(x => x.UserId == userId && x.EndDate >= startDate)
            .ToListAsync(ct);

        foreach (var existing in activeMemberships)
            existing.EndDate = startDate.AddDays(-1);

        var payment = new PaymentEntity
        {
            UserId = userId,
            Amount = amount,
            Method = PaymentMethod.Cash,
            Status = PaymentStatus.Completed,
        };
        ctx.Payments.Add(payment);

        var membership = new UserMembershipEntity
        {
            UserId = userId,
            MembershipPlanId = plan.Id,
            StartDate = startDate,
            EndDate = endDate,
        };
        ctx.UserMemberships.Add(membership);

        await ctx.SaveChangesAsync(ct);

        return new PurchaseMembershipPlanResultDto
        {
            UserMembershipId = membership.Id,
            PaymentId = payment.Id,
            MembershipPlanId = plan.Id,
            PlanName = plan.Name,
            AmountPaid = amount,
            StartDate = startDate,
            EndDate = endDate,
            PaymentMethod = payment.Method,
            PaymentStatus = payment.Status,
        };
    }

    private static decimal CalculateFinalPrice(decimal price, decimal discountPercentage)
    {
        var discount = price * discountPercentage / 100m;
        return Math.Round(price - discount, 2, MidpointRounding.AwayFromZero);
    }
}
