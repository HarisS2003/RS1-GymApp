using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.TrainingRequests.Commands.Create;

public sealed class CreateTrainingRequestCommandValidator : AbstractValidator<CreateTrainingRequestCommand>
{
    public CreateTrainingRequestCommandValidator()
    {
        RuleFor(x => x.TrainerPublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("TrainerPublicId must be a valid GUID.");
    }
}
