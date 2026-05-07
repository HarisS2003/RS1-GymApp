using Market.Domain.Common;

namespace Market.Domain.Entities;

public class MembershipPlanEntity : BaseEntity
{
    public int GymId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }

    public GymEntity? Gym { get; set; }
    public ICollection<UserMembershipEntity> UserMemberships { get; set; } = new List<UserMembershipEntity>();
}
