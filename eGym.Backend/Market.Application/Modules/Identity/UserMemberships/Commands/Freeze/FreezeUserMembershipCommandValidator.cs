using Market.Shared.Validation;

namespace Market.Application.Modules.Identity.UserMemberships.Commands.Freeze;

public sealed class FreezeUserMembershipCommandValidator : AbstractValidator<FreezeUserMembershipCommand>
{
    public FreezeUserMembershipCommandValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("PublicId must be a valid GUID.");
    }
}
