namespace Market.Infrastructure.Database.Configurations.MembershipEvents;

public class MembershipEventEntityConfiguration : IEntityTypeConfiguration<MembershipEvent>
{
    public void Configure(EntityTypeBuilder<MembershipEvent> builder)
    {
        builder.ToTable("MembershipEvents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EventData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.UserMembership)
            .WithMany()
            .HasForeignKey(x => x.UserMembershipId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserMembershipId);
        builder.HasIndex(x => new { x.UserMembershipId, x.CreatedAt });
    }
}
