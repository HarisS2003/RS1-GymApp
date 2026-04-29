using eGymSystem.Infrastructure.Database;
using eGymSystem.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace eGymSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            if (env.IsEnvironment("IntegrationTests"))
            {
                options.UseInMemoryDatabase("eGymSystemIntegrationTestsDb");
                return;
            }

            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.Main;
            options.UseSqlServer(cs);
        });

        services.AddHttpContextAccessor();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
