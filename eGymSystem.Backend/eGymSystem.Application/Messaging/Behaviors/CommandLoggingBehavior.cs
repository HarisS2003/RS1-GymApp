using eGymSystem.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace eGymSystem.Application.Messaging.Behaviors;

internal sealed class CommandLoggingBehavior<TCommand, TResponse>(ILogger<CommandLoggingBehavior<TCommand, TResponse>> logger)
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : class, ICommand<TResponse>
{
    public async Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;
        logger.LogInformation("Handling command {CommandName}.", commandName);

        var response = await next();

        logger.LogInformation("Handled command {CommandName}.", commandName);
        return response;
    }
}
