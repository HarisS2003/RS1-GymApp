using eGymSystem.Application.Abstractions.Messaging;
using eGymSystem.Application.Abstractions.Validation;

namespace eGymSystem.Application.Messaging.Behaviors;

internal sealed class CommandValidationBehavior<TCommand, TResponse>(IEnumerable<ICommandValidator<TCommand>> validators)
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : class, ICommand<TResponse>
{
    public Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var errors = validators
            .SelectMany(v => v.Validate(command))
            .Distinct()
            .ToArray();

        if (errors.Length != 0)
        {
            throw new CommandValidationException(errors);
        }

        return next();
    }
}
