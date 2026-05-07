namespace Market.Application.Modules.Catalog.MembershipPlans.Commands.Update;

public sealed class UpdateMembershipPlanCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int GymId { get; set; }
}
