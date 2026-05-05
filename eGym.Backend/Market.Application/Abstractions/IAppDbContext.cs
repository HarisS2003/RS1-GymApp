using Market.Domain.Entities;

namespace Market.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<UserEntity> Users { get; }
    DbSet<RoleEntity> Roles { get; }
    DbSet<GymEntity> Gyms { get; }
    DbSet<TrainerEntity> Trainers { get; }
    DbSet<MembershipPlanEntity> MembershipPlans { get; }
    DbSet<UserMembershipEntity> UserMemberships { get; }
    DbSet<PaymentEntity> Payments { get; }
    DbSet<ProductEntity> Products { get; }
    DbSet<BasketEntity> Baskets { get; }
    DbSet<BasketItemEntity> BasketItems { get; }
    DbSet<OrderEntity> Orders { get; }
    DbSet<OrderItemEntity> OrderItems { get; }
    DbSet<TrainingEntity> Trainings { get; }
    DbSet<TrainingParticipantEntity> TrainingParticipants { get; }
    DbSet<TrainingRequestEntity> TrainingRequests { get; }
    DbSet<NotificationEntity> Notifications { get; }
    DbSet<HashtagTagEntity> HashtagTags { get; }
    DbSet<ReviewEntity> Reviews { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
