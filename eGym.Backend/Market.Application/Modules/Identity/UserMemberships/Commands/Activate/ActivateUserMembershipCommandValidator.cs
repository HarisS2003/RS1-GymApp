using Market.Shared.Validation;

namespace Market.Application.Modules.Identity.UserMemberships.Commands.Activate;

public sealed class ActivateUserMembershipCommandValidator : AbstractValidator<ActivateUserMembershipCommand>
{
    public ActivateUserMembershipCommandValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("PublicId must be a valid GUID.");
    }
}
