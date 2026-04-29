using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace eGymSystem.Infrastructure;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, IHostEnvironment env)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Database.DatabaseContext>();

        if (env.IsEnvironment("IntegrationTests"))
        {
            await db.Database.EnsureCreatedAsync();
            return;
        }

        await db.Database.EnsureCreatedAsync();
    }
}
