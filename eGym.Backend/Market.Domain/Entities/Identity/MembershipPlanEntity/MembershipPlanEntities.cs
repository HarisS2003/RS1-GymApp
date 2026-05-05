using Market.Domain.Common;

namespace Market.Domain.Entities;

public class MembershipPlanEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }

    public ICollection<UserMembershipEntity> UserMemberships { get; set; } = new List<UserMembershipEntity>();
}
