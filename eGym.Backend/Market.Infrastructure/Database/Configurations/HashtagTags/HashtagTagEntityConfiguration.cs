namespace Market.Infrastructure.Database.Configurations.HashtagTags;

public class HashtagTagEntityConfiguration : IEntityTypeConfiguration<HashtagTagEntity>
{
    public void Configure(EntityTypeBuilder<HashtagTagEntity> builder)
    {
        builder
            .ToTable("HashtagTags");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}
