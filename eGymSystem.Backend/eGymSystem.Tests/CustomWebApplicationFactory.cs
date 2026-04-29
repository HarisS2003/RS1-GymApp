namespace eGymSystem.Tests;

public sealed class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
    }
}
