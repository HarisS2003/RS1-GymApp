namespace Market.Infrastructure.Database.Configurations.Notifications;

public class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder
            .ToTable("Notifications");

        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder
            .Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.HashtagTag)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.HashtagTagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
