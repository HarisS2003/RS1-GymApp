using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Trainings.Queries.List;

public sealed class ListTrainingsQueryValidator : AbstractValidator<ListTrainingsQuery>
{
    public ListTrainingsQueryValidator()
    {
        RuleFor(x => x.TrainerPublicId)
            .Must(PublicIdValidator.IsValidWhenProvided)
            .WithMessage("TrainerPublicId must be a valid GUID.")
            .When(x => !string.IsNullOrWhiteSpace(x.TrainerPublicId));
    }
}
