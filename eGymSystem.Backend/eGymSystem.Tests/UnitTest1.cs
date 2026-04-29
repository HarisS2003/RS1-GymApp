using eGymSystem.Application;
using eGymSystem.Application.Abstractions.Messaging;
using eGymSystem.Application.Abstractions.Validation;
using eGymSystem.Application.Modules.Training.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace eGymSystem.Tests;

public class CommandPipelineTests
{
    [Fact]
    public async Task Send_ValidCommand_ReturnsPendingResult()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        using var provider = services.BuildServiceProvider();

        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();
        var command = new CreateTrainingRequestCommand(10, 20, DateTime.UtcNow.AddHours(2));

        var result = await dispatcher.Send(command);

        Assert.True(result.RequestId > 0);
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task Send_InvalidCommand_ThrowsValidationException()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        using var provider = services.BuildServiceProvider();

        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();
        var command = new CreateTrainingRequestCommand(0, 0, DateTime.UtcNow.AddMinutes(-5));

        var act = () => dispatcher.Send(command);

        var exception = await Assert.ThrowsAsync<CommandValidationException>(act);
        Assert.NotEmpty(exception.Errors);
    }
}