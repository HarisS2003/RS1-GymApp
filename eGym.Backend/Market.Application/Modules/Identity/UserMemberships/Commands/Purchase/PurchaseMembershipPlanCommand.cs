namespace Market.Application.Modules.Identity.UserMemberships.Commands.Purchase;

public sealed class PurchaseMembershipPlanCommand : IRequest<PurchaseMembershipPlanResultDto>
{
    public int MembershipPlanId { get; set; }
    /// <summary>Only <see cref="PaymentMethod.Cash"/> is supported for now.</summary>
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
}
