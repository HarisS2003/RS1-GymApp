using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Trainings.Commands.Create;

public sealed class CreateTrainingCommandValidator : AbstractValidator<CreateTrainingCommand>
{
    public CreateTrainingCommandValidator()
    {
        RuleFor(x => x.TrainerPublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("TrainerPublicId must be a valid GUID.");
        RuleFor(x => x.Capacity).GreaterThan(0);
    }
}
