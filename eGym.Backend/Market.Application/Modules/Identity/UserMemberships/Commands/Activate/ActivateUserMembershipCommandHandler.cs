using Market.Application.Modules.Identity.UserMemberships.Services;

namespace Market.Application.Modules.Identity.UserMemberships.Commands.Activate;

public sealed class ActivateUserMembershipCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<ActivateUserMembershipCommand, Unit>
{
    public async Task<Unit> Handle(ActivateUserMembershipCommand request, CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var membership = await (
            from m in ctx.UserMemberships
            join u in ctx.Users on m.UserId equals u.Id
            where m.PublicId == request.PublicId && u.GymId == currentGymId
            select m
        ).FirstOrDefaultAsync(ct)
            ?? throw new MarketNotFoundException("User membership not found.");

        var events = await ctx.MembershipEvents.AsNoTracking()
            .Where(x => x.UserMembershipId == membership.Id)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        var state = MembershipEventReplayer.Replay(events, now);

        if (!state.Exists)
            throw new MarketBusinessRuleException("MEMBERSHIP_NOT_INITIALIZED", "Membership has no event history.");

        if (!state.IsFrozen)
            throw new MarketBusinessRuleException("MEMBERSHIP_NOT_FROZEN", "Membership is not frozen.");

        if (!state.EndDate.HasValue || state.EndDate.Value.Date < now.Date)
            throw new MarketBusinessRuleException("MEMBERSHIP_EXPIRED", "Cannot activate an expired membership.");

        MembershipEventRecorder.Append(
            ctx,
            membership.Id,
            MembershipEventTypes.Activated,
            new
            {
                reason = request.Reason ?? "admin_activate",
                activatedAt = now,
            },
            now);

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }

    private async Task<int> GetCurrentGymIdAsync(CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var gymId = await ctx.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.GymId)
            .FirstOrDefaultAsync(ct);

        return gymId == 0
            ? throw new MarketNotFoundException("User not found.")
            : gymId;
    }
}
