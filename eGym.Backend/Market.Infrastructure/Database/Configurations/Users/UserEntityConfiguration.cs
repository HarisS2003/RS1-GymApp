namespace Market.Infrastructure.Database.Configurations.Users;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .ToTable("Users");

        builder
            .Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(512);

        builder
            .Property(x => x.PasswordHash)
            .IsRequired();

        builder
            .HasIndex(x => x.Email)
            .IsUnique();

        builder
            .HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Gym)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.GymId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
