namespace Market.Application.Modules.Identity.UserMemberships.Queries.GetMyActive;

public sealed class GetMyActiveUserMembershipQueryDto
{
    public required string PublicId { get; init; }
    public required int MembershipPlanId { get; init; }
    public required string PlanName { get; init; }
    public required int DurationDays { get; init; }
    public required decimal Price { get; init; }
    public required decimal DiscountPercentage { get; init; }
    public required decimal FinalPrice { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
}
