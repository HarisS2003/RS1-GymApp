namespace Market.Infrastructure.Database.Configurations.MembershipPlans;

public class MembershipPlanEntityConfiguration : IEntityTypeConfiguration<MembershipPlanEntity>
{
    public void Configure(EntityTypeBuilder<MembershipPlanEntity> builder)
    {
        builder
            .ToTable("MembershipPlans");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        builder
            .Property(x => x.DiscountPercentage)
            .HasPrecision(5, 2);
    }
}
