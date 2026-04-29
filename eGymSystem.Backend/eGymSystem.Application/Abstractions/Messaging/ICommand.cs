namespace eGymSystem.Application.Abstractions.Messaging;

public interface ICommand<out TResponse>
{
}

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}

public delegate Task<TResponse> CommandHandlerDelegate<TResponse>();

public interface ICommandBehavior<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}

public interface ICommandDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}
