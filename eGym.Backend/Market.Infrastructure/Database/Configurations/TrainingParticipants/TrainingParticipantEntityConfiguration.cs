namespace Market.Infrastructure.Database.Configurations.TrainingParticipants;

public class TrainingParticipantEntityConfiguration : IEntityTypeConfiguration<TrainingParticipantEntity>
{
    public void Configure(EntityTypeBuilder<TrainingParticipantEntity> builder)
    {
        builder
            .ToTable("TrainingParticipants");

        builder
            .HasIndex(x => new { x.TrainingId, x.UserId })
            .IsUnique();

        builder
            .HasOne(x => x.Training)
            .WithMany(x => x.TrainingParticipants)
            .HasForeignKey(x => x.TrainingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.TrainingParticipants)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
