namespace eGymSystem.Infrastructure.Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<TrainingRequestEntity> TrainingRequests => Set<TrainingRequestEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
}
