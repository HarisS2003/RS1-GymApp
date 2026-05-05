namespace Market.Infrastructure.Database.Configurations.Baskets;

public class BasketEntityConfiguration : IEntityTypeConfiguration<BasketEntity>
{
    public void Configure(EntityTypeBuilder<BasketEntity> builder)
    {
        builder
            .ToTable("Basket");

        builder
            .HasOne(x => x.User)
            .WithOne(x => x.Basket)
            .HasForeignKey<BasketEntity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.UserId)
            .IsUnique();
    }
}
