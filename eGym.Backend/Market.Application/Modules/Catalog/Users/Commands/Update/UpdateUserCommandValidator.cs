using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Users.Commands.Update;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(p => BosnianPhoneNumberValidator.IsValid(p, out _))
            .WithMessage("Invalid Bosnian phone number format.");
        RuleFor(x => x.RoleId).GreaterThan(0);
        RuleFor(x => x.GymId).GreaterThan(0);
    }
}
