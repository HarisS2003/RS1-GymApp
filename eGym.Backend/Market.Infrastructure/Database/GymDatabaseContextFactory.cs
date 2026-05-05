using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Market.Infrastructure.Database;

public class GymDatabaseContextFactory : IDesignTimeDbContextFactory<GymDatabaseContext>
{
    public GymDatabaseContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var apiPath = Path.GetFullPath(Path.Combine(basePath, "..", "Market.API"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = configuration.GetSection("ConnectionStrings")["Main"]
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:Main.");

        var optionsBuilder = new DbContextOptionsBuilder<GymDatabaseContext>();
        optionsBuilder.UseSqlServer(cs);

        return new GymDatabaseContext(optionsBuilder.Options);
    }
}
