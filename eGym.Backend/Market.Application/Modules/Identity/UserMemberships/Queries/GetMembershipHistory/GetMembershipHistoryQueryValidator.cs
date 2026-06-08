using Market.Shared.Validation;

namespace Market.Application.Modules.Identity.UserMemberships.Queries.GetMembershipHistory;

public sealed class GetMembershipHistoryQueryValidator : AbstractValidator<GetMembershipHistoryQuery>
{
    public GetMembershipHistoryQueryValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("PublicId must be a valid GUID.");
    }
}
