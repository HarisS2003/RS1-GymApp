namespace Market.Application.Modules.Catalog.Trainings.Queries.GetById;

public sealed class GetTrainingByIdQuery : IRequest<GetTrainingByIdQueryDto>
{
    public int Id { get; set; }
}
