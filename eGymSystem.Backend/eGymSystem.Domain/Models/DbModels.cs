namespace eGymSystem.Domain.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}

public enum MembershipDurationType { Daily = 1, Weekly = 2, Monthly = 3 }
public enum UserMembershipStatusType { Pending = 1, Active = 2, Expired = 3, Cancelled = 4 }
public enum PaymentStatusType { Pending = 1, Completed = 2, Failed = 3 }
public enum PaymentType { Membership = 1, Product = 2 }
public enum TrainingStatusType { Pending = 1, Approved = 2, Rejected = 3, Cancelled = 4 }
public enum OrderStatusType { Pending = 1, Paid = 2, Cancelled = 3, Completed = 4 }

public sealed class RoleEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

public sealed class GymEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public sealed class UserEntity : BaseEntity
{
    public int GymId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public GymEntity? Gym { get; set; }
}

public sealed class TrainerEntity : BaseEntity
{
    public int UserId { get; set; }
    public int GymId { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public UserEntity? User { get; set; }
    public GymEntity? Gym { get; set; }
}

public sealed class MembershipPlanEntity : BaseEntity
{
    public int GymId { get; set; }
    public string Name { get; set; } = string.Empty;
    public MembershipDurationType DurationType { get; set; }
    public decimal Discount { get; set; }
    public decimal Price { get; set; }
}

public sealed class UserMembershipEntity : BaseEntity
{
    public int UserId { get; set; }
    public int PlanId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public UserMembershipStatusType Status { get; set; }
}

public sealed class PaymentEntity : BaseEntity
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatusType Status { get; set; }
    public PaymentType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class ProductEntity : BaseEntity
{
    public int GymId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public sealed class BasketEntity : BaseEntity
{
    public int UserId { get; set; }
}

public sealed class BasketItemEntity
{
    public int BasketId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public sealed class OrderEntity : BaseEntity
{
    public int UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatusType Status { get; set; }
}

public sealed class OrderItemEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }
}

public sealed class TrainingEntity : BaseEntity
{
    public int TrainerId { get; set; }
    public int UserId { get; set; }
    public int GymId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TrainingStatusType Status { get; set; }
}

public sealed class TrainingRequestEntity : BaseEntity
{
    public int UserId { get; set; }
    public int TrainerId { get; set; }
    public DateTime RequestedTimeUtc { get; set; }
    public TrainingStatusType Status { get; set; }
}

public sealed class TrainingParticipantEntity
{
    public int TrainingId { get; set; }
    public int UserId { get; set; }
}

public sealed class NotificationEntity : BaseEntity
{
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}
