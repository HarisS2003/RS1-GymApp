using eGymSystem.API;
using eGymSystem.API.Endpoints;
using eGymSystem.API.Middleware;
using eGymSystem.Application;
using eGymSystem.Infrastructure;
using Serilog;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, services, cfg) =>
            {
                cfg.ReadFrom.Configuration(ctx.Configuration)
                   .ReadFrom.Services(services)
                   .Enrich.FromLogContext()
                   .Enrich.WithThreadId()
                   .Enrich.WithProcessId()
                   .Enrich.WithMachineName();
            });

            builder.Logging.ClearProviders();

            builder.Services
                .AddAPI(builder.Configuration, builder.Environment)
                .AddInfrastructure(builder.Configuration, builder.Environment)
                .AddApplication();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapCrudSkeletonEndpoints();

            await app.Services.InitializeDatabaseAsync(app.Environment);
            app.Run();
        }
        catch (HostAbortedException)
        {
            Log.Information("Host aborted by tooling.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "eGymSystem API terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
