namespace Market.Application.Modules.Identity.UserMemberships.Commands.Purchase;

public sealed class PurchaseMembershipPlanResultDto
{
    public required string PublicId { get; init; }
    public required int PaymentId { get; init; }
    public required int MembershipPlanId { get; init; }
    public required string PlanName { get; init; }
    public required decimal AmountPaid { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required PaymentMethod PaymentMethod { get; init; }
    public required PaymentStatus PaymentStatus { get; init; }
}
