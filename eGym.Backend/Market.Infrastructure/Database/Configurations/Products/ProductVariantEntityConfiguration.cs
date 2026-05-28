namespace Market.Infrastructure.Database.Configurations.Products;

public class ProductVariantEntityConfiguration : IEntityTypeConfiguration<ProductVariantEntity>
{
    public void Configure(EntityTypeBuilder<ProductVariantEntity> builder)
    {
        builder.ToTable("ProductVariants");

        builder.Property(x => x.Size)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Color)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.ProductId);

        builder.HasIndex(x => new { x.ProductId, x.Size, x.Color })
            .IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany(x => x.ProductVariants)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
