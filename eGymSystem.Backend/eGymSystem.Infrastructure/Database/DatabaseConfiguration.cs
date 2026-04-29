namespace eGymSystem.Infrastructure.Database;

public sealed class DatabaseConfiguration : IEntityTypeConfiguration<TrainingRequestEntity>
{
    public void Configure(EntityTypeBuilder<TrainingRequestEntity> builder)
    {
        builder.ToTable("TrainingRequests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RequestedTimeUtc).IsRequired();
    }
}
