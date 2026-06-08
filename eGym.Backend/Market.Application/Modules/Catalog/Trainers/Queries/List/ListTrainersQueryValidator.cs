using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Trainers.Queries.List;

public sealed class ListTrainersQueryValidator : AbstractValidator<ListTrainersQuery>
{
    public ListTrainersQueryValidator()
    {
        RuleFor(x => x.UserPublicId)
            .Must(PublicIdValidator.IsValid)
            .WithMessage("UserPublicId must be a valid GUID.")
            .When(x => !string.IsNullOrWhiteSpace(x.UserPublicId));
    }
}
