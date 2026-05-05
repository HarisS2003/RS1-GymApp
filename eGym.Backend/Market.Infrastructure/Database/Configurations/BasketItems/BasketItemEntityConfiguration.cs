namespace Market.Infrastructure.Database.Configurations.BasketItems;

public class BasketItemEntityConfiguration : IEntityTypeConfiguration<BasketItemEntity>
{
    public void Configure(EntityTypeBuilder<BasketItemEntity> builder)
    {
        builder
            .ToTable("BasketItems");

        builder
            .HasIndex(x => new { x.BasketId, x.ProductId })
            .IsUnique();

        builder
            .HasOne(x => x.Basket)
            .WithMany(x => x.BasketItems)
            .HasForeignKey(x => x.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.BasketItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
