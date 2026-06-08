namespace Market.Infrastructure.Database.Configurations.UserMemberships;

public class UserMembershipEntityConfiguration : IEntityTypeConfiguration<UserMembershipEntity>
{
    public void Configure(EntityTypeBuilder<UserMembershipEntity> builder)
    {
        builder
            .ToTable("UserMemberships");

        builder
            .Property(x => x.PublicId)
            .IsRequired()
            .HasMaxLength(36);

        builder
            .HasIndex(x => x.PublicId)
            .IsUnique();

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.UserMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.MembershipPlan)
            .WithMany(x => x.UserMemberships)
            .HasForeignKey(x => x.MembershipPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
