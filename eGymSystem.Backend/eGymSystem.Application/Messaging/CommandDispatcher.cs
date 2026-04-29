using eGymSystem.Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace eGymSystem.Application.Messaging;

internal sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();
        var method = typeof(CommandDispatcher)
            .GetMethod(nameof(SendInternal), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .MakeGenericMethod(commandType, typeof(TResponse));

        return (Task<TResponse>)method.Invoke(this, [command, cancellationToken])!;
    }

    private Task<TResponse> SendInternal<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        var behaviors = serviceProvider
            .GetServices<ICommandBehavior<TCommand, TResponse>>()
            .Reverse()
            .ToArray();

        CommandHandlerDelegate<TResponse> next = () => handler.Handle(command, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var currentNext = next;
            next = () => behavior.Handle(command, currentNext, cancellationToken);
        }

        return next();
    }
}
