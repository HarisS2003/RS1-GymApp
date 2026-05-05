namespace Market.Infrastructure.Database.Configurations.TrainingRequests;

public class TrainingRequestEntityConfiguration : IEntityTypeConfiguration<TrainingRequestEntity>
{
    public void Configure(EntityTypeBuilder<TrainingRequestEntity> builder)
    {
        builder
            .ToTable("TrainingRequests");

        builder
            .Property(x => x.Status)
            .HasConversion<int>();

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.TrainingRequests)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Trainer)
            .WithMany(x => x.TrainingRequests)
            .HasForeignKey(x => x.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
