using eGymSystem.Application.Abstractions.Messaging;

namespace eGymSystem.Application.Abstractions.Validation;

public interface ICommandValidator<in TCommand>
    where TCommand : class
{
    IReadOnlyCollection<string> Validate(TCommand command);
}

public sealed class CommandValidationException : Exception
{
    public CommandValidationException(IReadOnlyCollection<string> errors)
        : base("Command validation failed.")
    {
        Errors = errors;
    }

    public IReadOnlyCollection<string> Errors { get; }
}
