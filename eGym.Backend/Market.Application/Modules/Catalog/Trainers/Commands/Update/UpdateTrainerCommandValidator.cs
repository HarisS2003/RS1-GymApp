using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Trainers.Commands.Update;

public sealed class UpdateTrainerCommandValidator : AbstractValidator<UpdateTrainerCommand>
{
    public UpdateTrainerCommandValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("PublicId must be a valid GUID.");
        RuleFor(x => x.UserPublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("UserPublicId must be a valid GUID.");
        RuleFor(x => x.GymId).GreaterThan(0);
        RuleFor(x => x.Bio).NotEmpty();
        RuleFor(x => x.ExperienceYears).GreaterThanOrEqualTo(0);
    }
}
