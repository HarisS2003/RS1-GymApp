using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Trainers.Queries.GetById;

public sealed class GetTrainerByIdQueryValidator : AbstractValidator<GetTrainerByIdQuery>
{
    public GetTrainerByIdQueryValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty()
            .Must(PublicIdValidator.IsValid)
            .WithMessage("PublicId must be a valid GUID.");
    }
}
