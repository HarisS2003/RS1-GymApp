namespace Market.Application.Modules.Catalog.MembershipPlans.Commands.Create;

public class CreateMembershipPlanCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int GymId { get; set; }
}
