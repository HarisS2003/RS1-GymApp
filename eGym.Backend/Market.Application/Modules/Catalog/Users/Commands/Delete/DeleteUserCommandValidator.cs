using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Users.Commands.Delete;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("PublicId must be a valid GUID.");
    }
}
