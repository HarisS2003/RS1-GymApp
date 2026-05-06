namespace Market.Application.Modules.Catalog.Gyms.Queries.GetById;

public class GetGymByIdQuery : IRequest<GetGymByIdQueryDto>
{
    public int Id { get; set; }
}
