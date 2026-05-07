using Market.Domain.Common;

namespace Market.Domain.Entities;

public class GymEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    public ICollection<TrainerEntity> Trainers { get; set; } = new List<TrainerEntity>();
    public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    public ICollection<MembershipPlanEntity> MembershipPlans { get; set; } = new List<MembershipPlanEntity>();
}
