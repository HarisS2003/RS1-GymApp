using Market.Application.Modules.Identity.UserMemberships.Services;

namespace Market.Application.Modules.Identity.UserMemberships.Commands.Freeze;

public sealed class FreezeUserMembershipCommandHandler(IAppDbContext ctx)
    : IRequestHandler<FreezeUserMembershipCommand, Unit>
{
    public async Task<Unit> Handle(FreezeUserMembershipCommand request, CancellationToken ct)
    {
        var membership = await ctx.UserMemberships
            .FirstOrDefaultAsync(x => x.Id == request.UserMembershipId, ct)
            ?? throw new MarketNotFoundException("User membership not found.");

        var events = await ctx.MembershipEvents.AsNoTracking()
            .Where(x => x.UserMembershipId == request.UserMembershipId)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        var state = MembershipEventReplayer.Replay(events, now);

        if (!state.Exists)
        {
            throw new MarketBusinessRuleException("MEMBERSHIP_NOT_INITIALIZED", "Membership has no event history.");
        }

        if (state.IsFrozen)
        {
            throw new MarketBusinessRuleException("MEMBERSHIP_ALREADY_FROZEN", "Membership is already frozen.");
        }

        if (!state.EndDate.HasValue || state.EndDate.Value.Date < now.Date)
        {
            throw new MarketBusinessRuleException("MEMBERSHIP_EXPIRED", "Cannot freeze an expired membership.");
        }

        MembershipEventRecorder.Append(
            ctx,
            membership.Id,
            MembershipEventTypes.Frozen,
            new
            {
                reason = request.Reason ?? "admin_freeze",
                frozenAt = now,
            },
            now);

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
