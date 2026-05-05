using Market.Domain.Common;

namespace Market.Domain.Entities;

public class UserMembershipEntity : BaseEntity
{
    public int UserId { get; set; }
    public int MembershipPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public UserEntity? User { get; set; }
    public MembershipPlanEntity? MembershipPlan { get; set; }
}
