namespace Market.Infrastructure.Database.Configurations.Products;

public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder
            .ToTable("Products");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        builder
            .HasOne(x => x.Gym)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.GymId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
