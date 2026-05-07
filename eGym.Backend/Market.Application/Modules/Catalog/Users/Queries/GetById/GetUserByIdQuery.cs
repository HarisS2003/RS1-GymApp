namespace Market.Application.Modules.Catalog.Users.Queries.GetById;

public class GetUserByIdQuery : IRequest<GetUserByIdQueryDto>
{
    public int Id { get; set; }
}
