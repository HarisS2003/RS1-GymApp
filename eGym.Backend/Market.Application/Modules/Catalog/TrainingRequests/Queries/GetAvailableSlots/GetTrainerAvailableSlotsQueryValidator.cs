using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

public sealed class GetTrainerAvailableSlotsQueryValidator : AbstractValidator<GetTrainerAvailableSlotsQuery>
{
    public GetTrainerAvailableSlotsQueryValidator()
    {
        RuleFor(x => x.TrainerPublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("TrainerPublicId must be a valid GUID.");
    }
}
