namespace Market.Infrastructure.Database.Configurations.Orders;

public class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder
            .ToTable("Orders");

        builder
            .Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder
            .Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
