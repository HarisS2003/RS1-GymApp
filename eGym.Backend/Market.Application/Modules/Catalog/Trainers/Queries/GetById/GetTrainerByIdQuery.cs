namespace Market.Application.Modules.Catalog.Trainers.Queries.GetById;

public sealed class GetTrainerByIdQuery : IRequest<GetTrainerByIdQueryDto>
{
    public int Id { get; set; }
}
