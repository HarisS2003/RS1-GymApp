namespace Market.Infrastructure.Database.Configurations.Trainers;

public class TrainerEntityConfiguration : IEntityTypeConfiguration<TrainerEntity>
{
    public void Configure(EntityTypeBuilder<TrainerEntity> builder)
    {
        builder
            .ToTable("Trainers");

        builder
            .Property(x => x.PublicId)
            .IsRequired()
            .HasMaxLength(36);

        builder
            .HasIndex(x => x.PublicId)
            .IsUnique();

        builder
            .Property(x => x.Bio)
            .HasMaxLength(2000);

        builder
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<TrainerEntity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.UserId)
            .IsUnique();

        builder
            .HasOne(x => x.Gym)
            .WithMany(x => x.Trainers)
            .HasForeignKey(x => x.GymId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
