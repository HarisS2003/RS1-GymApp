namespace Market.Application.Modules.Identity.UserMemberships.Queries.ListMyHistory;

public sealed class ListMyMembershipPurchaseHistoryQueryDto
{
    public required string PublicId { get; init; }
    public required string PlanName { get; init; }
    public required decimal AmountPaid { get; init; }
    public required DateTime PurchasedAt { get; init; }
    public required DateTime EndDate { get; init; }
    public required int DurationDays { get; init; }
    public required bool IsActive { get; init; }
}
