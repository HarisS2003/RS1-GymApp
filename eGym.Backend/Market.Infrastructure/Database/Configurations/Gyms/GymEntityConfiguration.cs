namespace Market.Infrastructure.Database.Configurations.Gyms;

public class GymEntityConfiguration : IEntityTypeConfiguration<GymEntity>
{
    public void Configure(EntityTypeBuilder<GymEntity> builder)
    {
        builder
            .ToTable("Gyms");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder
            .Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(250);

        builder
            .Property(x => x.City)
            .IsRequired()
            .HasMaxLength(120);
    }
}
