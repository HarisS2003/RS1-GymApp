using Market.Application.Abstractions;
using Market.Domain.Entities;
using Market.Infrastructure.Security;

namespace Market.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<GymEntity> Gyms => Set<GymEntity>();
    public DbSet<TrainerEntity> Trainers => Set<TrainerEntity>();
    public DbSet<MembershipPlanEntity> MembershipPlans => Set<MembershipPlanEntity>();
    public DbSet<UserMembershipEntity> UserMemberships => Set<UserMembershipEntity>();
    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<ProductVariantEntity> ProductVariants => Set<ProductVariantEntity>();
    public DbSet<BasketEntity> Baskets => Set<BasketEntity>();
    public DbSet<BasketItemEntity> BasketItems => Set<BasketItemEntity>();
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<TrainingEntity> Trainings => Set<TrainingEntity>();
    public DbSet<TrainingParticipantEntity> TrainingParticipants => Set<TrainingParticipantEntity>();
    public DbSet<TrainingRequestEntity> TrainingRequests => Set<TrainingRequestEntity>();
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();
    public DbSet<HashtagTagEntity> HashtagTags => Set<HashtagTagEntity>();
    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();

    private readonly TimeProvider _clock;
    private readonly AesEncryptionHelper _encryption;

    public DatabaseContext(
        DbContextOptions<DatabaseContext> options,
        TimeProvider clock,
        AesEncryptionHelper encryption)
        : base(options)
    {
        _clock = clock;
        _encryption = encryption;
    }
}
