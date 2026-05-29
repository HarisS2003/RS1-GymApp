using Market.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Database.Seeders;

public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        var users = await context.Users
            .OrderBy(u => u.Id)
            .ToListAsync();

        if (users.Count == 0)
        {
            return;
        }

        var changed = false;
        for (var i = 0; i < users.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(users[i].PhoneNumber)
                && BosnianPhoneNumberValidator.IsValid(users[i].PhoneNumber, out _))
            {
                continue;
            }

            // Unique Bosnian mobiles: +387 6X XXXXXX (8 digits after country code)
            var operatorCode = 60 + (i % 10);
            var subscriber = 100_000 + i;
            users[i].PhoneNumber = $"+387{operatorCode}{subscriber:D6}";
            changed = true;
        }

        if (changed)
        {
            await context.SaveChangesAsync();
        }
    }
}
