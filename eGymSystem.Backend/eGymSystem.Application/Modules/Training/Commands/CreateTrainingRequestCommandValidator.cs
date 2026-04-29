using eGymSystem.Application.Abstractions.Validation;

namespace eGymSystem.Application.Modules.Training.Commands;

internal sealed class CreateTrainingRequestCommandValidator : ICommandValidator<CreateTrainingRequestCommand>
{
    public IReadOnlyCollection<string> Validate(CreateTrainingRequestCommand command)
    {
        var errors = new List<string>();

        if (command.UserId <= 0)
        {
            errors.Add("UserId must be greater than 0.");
        }

        if (command.TrainerId <= 0)
        {
            errors.Add("TrainerId must be greater than 0.");
        }

        if (command.RequestedTimeUtc <= DateTime.UtcNow)
        {
            errors.Add("RequestedTimeUtc must be in future.");
        }

        return errors;
    }
}
