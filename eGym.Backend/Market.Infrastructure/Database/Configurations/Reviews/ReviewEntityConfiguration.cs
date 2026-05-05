namespace Market.Infrastructure.Database.Configurations.Reviews;

public class ReviewEntityConfiguration : IEntityTypeConfiguration<ReviewEntity>
{
    public void Configure(EntityTypeBuilder<ReviewEntity> builder)
    {
        builder
            .ToTable("Reviews");

        builder
            .Property(x => x.Comment)
            .HasMaxLength(2000);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Trainer)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
