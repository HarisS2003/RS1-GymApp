namespace Market.Infrastructure.Database.Configurations.Payments;

public class PaymentEntityConfiguration : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(EntityTypeBuilder<PaymentEntity> builder)
    {
        builder
            .ToTable("Payments");

        builder
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder
            .Property(x => x.Method)
            .HasConversion<int>();

        builder
            .Property(x => x.Status)
            .HasConversion<int>();

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
