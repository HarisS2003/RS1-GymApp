namespace Market.Application.Modules.Identity.UserMemberships.Queries.GetMembershipHistory;



public sealed class GetMembershipHistoryQueryDto

{

    public required string PublicId { get; init; }

    public required DateTime AsOfDate { get; init; }

    public required MembershipHistoryStateDto State { get; init; }

    public required IReadOnlyList<MembershipEventTimelineItemDto> Timeline { get; init; }

}



public sealed class MembershipHistoryStateDto

{

    public required bool HasMembership { get; init; }

    public string? UserPublicId { get; init; }

    public int? MembershipPlanId { get; init; }

    public string? PlanName { get; init; }

    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public string? PeriodDisplay { get; init; }

    public required string Status { get; init; }

    public required bool IsFrozen { get; init; }

}



public sealed class MembershipEventTimelineItemDto

{

    public required int Id { get; init; }

    public required string EventType { get; init; }

    public required string EventData { get; init; }

    public required DateTime CreatedAt { get; init; }

}

