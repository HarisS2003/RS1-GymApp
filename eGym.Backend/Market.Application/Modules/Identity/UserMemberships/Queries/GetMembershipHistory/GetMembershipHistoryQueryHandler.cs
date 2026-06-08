using Market.Application.Modules.Identity.UserMemberships.Services;

namespace Market.Application.Modules.Identity.UserMemberships.Queries.GetMembershipHistory;

public sealed class GetMembershipHistoryQueryHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<GetMembershipHistoryQuery, GetMembershipHistoryQueryDto>
{
    public async Task<GetMembershipHistoryQueryDto> Handle(
        GetMembershipHistoryQuery request,
        CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var membershipRow = await (
            from m in ctx.UserMemberships.AsNoTracking()
            join u in ctx.Users.AsNoTracking() on m.UserId equals u.Id
            join p in ctx.MembershipPlans.AsNoTracking() on m.MembershipPlanId equals p.Id into plans
            from p in plans.DefaultIfEmpty()
            where m.PublicId == request.PublicId && u.GymId == currentGymId
            select new
            {
                Membership = m,
                UserPublicId = u.PublicId,
                PlanName = p != null ? p.Name : "Membership",
            }
        ).FirstOrDefaultAsync(ct)
            ?? throw new MarketNotFoundException("User membership not found.");

        var userMembershipId = membershipRow.Membership.Id;

        var events = await ctx.MembershipEvents.AsNoTracking()
            .Where(x => x.UserMembershipId == userMembershipId)
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
            planName = membershipRow.PlanName;

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
            PublicId = membershipRow.Membership.PublicId,
            AsOfDate = asOfDate,
            State = await BuildStateAsync(ctx, replayed, planName, membershipRow.UserPublicId, ct),
            Timeline = timeline,
        };
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

    private static async Task<MembershipHistoryStateDto> BuildStateAsync(
        IAppDbContext ctx,
        MembershipReplayState replayed,
        string? planName,
        string membershipUserPublicId,
        CancellationToken ct)
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

        string? userPublicId = null;
        if (replayed.UserId > 0)
        {
            userPublicId = await ctx.Users.AsNoTracking()
                .Where(x => x.Id == replayed.UserId)
                .Select(x => x.PublicId)
                .FirstOrDefaultAsync(ct);
        }

        userPublicId ??= membershipUserPublicId;

        return new MembershipHistoryStateDto
        {
            HasMembership = true,
            UserPublicId = userPublicId,
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
