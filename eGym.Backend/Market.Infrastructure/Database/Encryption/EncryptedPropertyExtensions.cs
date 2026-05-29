using System.Reflection;
using Market.Domain.Attributes;
using Market.Infrastructure.Security;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Market.Infrastructure.Database.Encryption;

public static class EncryptedPropertyExtensions
{
    public static void ApplyEncryptedStringConverters(this ModelBuilder modelBuilder, AesEncryptionHelper encryption)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(encryption);

        var converter = new ValueConverter<string, string>(
            plain => encryption.Encrypt(plain),
            cipher => encryption.Decrypt(cipher));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType is null)
            {
                continue;
            }

            foreach (var property in clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType != typeof(string))
                {
                    continue;
                }

                if (!Attribute.IsDefined(property, typeof(EncryptedAttribute)))
                {
                    continue;
                }

                modelBuilder.Entity(clrType).Property(property.Name).HasConversion(converter);
            }
        }
    }
}
