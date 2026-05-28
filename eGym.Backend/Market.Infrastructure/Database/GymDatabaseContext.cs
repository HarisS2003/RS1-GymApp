using Market.Domain.Entities;

namespace Market.Infrastructure.Database;

public class GymDatabaseContext : DbContext
{
    public GymDatabaseContext(DbContextOptions<GymDatabaseContext> options) : base(options)
    {
    }

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

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        configurationBuilder.Properties<decimal?>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDatabaseContext).Assembly);
    }
}
