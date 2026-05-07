namespace Market.Application.Modules.Catalog.MembershipPlans.Queries.List;

public sealed class ListMembershipPlansQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int DurationDays { get; init; }
    public required decimal Price { get; set; }
    public required decimal DiscountPercentage { get; set; }
    public required int GymId { get; set; }
}
