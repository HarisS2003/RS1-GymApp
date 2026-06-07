using Market.Application.Modules.Identity.UserMemberships.Services;

namespace Market.Application.Modules.Identity.UserMemberships.Queries.GetMembershipHistory;

public sealed class GetMembershipHistoryQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetMembershipHistoryQuery, GetMembershipHistoryQueryDto>
{
    public async Task<GetMembershipHistoryQueryDto> Handle(
        GetMembershipHistoryQuery request,
        CancellationToken ct)
    {
        var membershipRow = await (
            from m in ctx.UserMemberships.AsNoTracking()
            join p in ctx.MembershipPlans.AsNoTracking() on m.MembershipPlanId equals p.Id into plans
            from p in plans.DefaultIfEmpty()
            where m.Id == request.UserMembershipId
            select new
            {
                Membership = m,
                PlanName = p != null ? p.Name : "Membership",
            }
        ).FirstOrDefaultAsync(ct)
            ?? throw new MarketNotFoundException("User membership not found.");

        var events = await ctx.MembershipEvents.AsNoTracking()
            .Where(x => x.UserMembershipId == request.UserMembershipId)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToListAsync(ct);

        var asOfDate = MembershipHistoryDateHelper.NormalizeAsOfDate(
            request.AsOfDate == default ? DateTime.UtcNow : request.AsOfDate);
        var asOfCalendarDate = MembershipHistoryDateHelper.AsOfCalendarDate(asOfDate);

        var replayed = MembershipEventReplayer.Replay(events, asOfDate);
        if (!replayed.Exists && events.Count == 0)
        {
            replayed = MembershipEventReplayer.FromMembershipSnapshot(
                membershipRow.Membership.UserId,
                membershipRow.Membership.MembershipPlanId,
                membershipRow.PlanName,
                membershipRow.Membership.StartDate,
                membershipRow.Membership.EndDate,
                asOfDate);
        }

        var planName = replayed.PlanName;
        if (replayed.Exists && string.IsNullOrWhiteSpace(planName))
        {
            planName = membershipRow.PlanName;
        }

        if (replayed.Exists && string.IsNullOrWhiteSpace(planName) && replayed.MembershipPlanId > 0)
        {
            planName = await ctx.MembershipPlans.AsNoTracking()
                .Where(x => x.Id == replayed.MembershipPlanId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(ct);
        }

        var timeline = events
            .Where(x => x.CreatedAt.Date <= asOfCalendarDate)
            .Select(x => new MembershipEventTimelineItemDto
            {
                Id = x.Id,
                EventType = x.EventType,
                EventData = x.EventData,
                CreatedAt = x.CreatedAt,
            })
            .ToList();

        return new GetMembershipHistoryQueryDto
        {
            UserMembershipId = request.UserMembershipId,
            AsOfDate = asOfDate,
            State = BuildState(replayed, planName),
            Timeline = timeline,
        };
    }

    private static MembershipHistoryStateDto BuildState(MembershipReplayState replayed, string? planName)
    {
        if (!replayed.Exists)
        {
            return new MembershipHistoryStateDto
            {
                HasMembership = false,
                Status = "None",
                IsFrozen = false,
            };
        }

        var hasValidPeriod = replayed.StartDate.HasValue
            && replayed.EndDate.HasValue
            && replayed.StartDate.Value.Year > 1
            && replayed.EndDate.Value.Year > 1;

        string? periodDisplay = hasValidPeriod
            ? $"{replayed.StartDate!.Value:dd.MM.yyyy} – {replayed.EndDate!.Value:dd.MM.yyyy}"
            : null;

        return new MembershipHistoryStateDto
        {
            HasMembership = true,
            UserId = replayed.UserId > 0 ? replayed.UserId : null,
            MembershipPlanId = replayed.MembershipPlanId > 0 ? replayed.MembershipPlanId : null,
            PlanName = string.IsNullOrWhiteSpace(planName) ? null : planName,
            StartDate = hasValidPeriod ? replayed.StartDate : null,
            EndDate = hasValidPeriod ? replayed.EndDate : null,
            PeriodDisplay = periodDisplay,
            Status = replayed.Status,
            IsFrozen = replayed.IsFrozen,
        };
    }
}
