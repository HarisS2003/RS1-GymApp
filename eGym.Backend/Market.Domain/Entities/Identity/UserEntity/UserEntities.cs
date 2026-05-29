using Market.Domain.Attributes;
using Market.Domain.Common;

namespace Market.Domain.Entities;

public class UserEntity : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Encrypted]
    public string PhoneNumber { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int GymId { get; set; }

    public RoleEntity? Role { get; set; }
    public GymEntity? Gym { get; set; }
    public BasketEntity? Basket { get; set; }

    public ICollection<TrainingRequestEntity> TrainingRequests { get; set; } = new List<TrainingRequestEntity>();
    public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
    public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
    public ICollection<UserMembershipEntity> UserMemberships { get; set; } = new List<UserMembershipEntity>();
    public ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();
    public ICollection<TrainingParticipantEntity> TrainingParticipants { get; set; } = new List<TrainingParticipantEntity>();
}
