namespace Market.Infrastructure.Database.Configurations.Trainings;

public class TrainingEntityConfiguration : IEntityTypeConfiguration<TrainingEntity>
{
    public void Configure(EntityTypeBuilder<TrainingEntity> builder)
    {
        builder
            .ToTable("Trainings");

        builder
            .Property(x => x.Type)
            .HasConversion<int>();

        builder
            .Property(x => x.Description)
            .HasMaxLength(1000);

        builder
            .HasOne(x => x.Trainer)
            .WithMany(x => x.Trainings)
            .HasForeignKey(x => x.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
