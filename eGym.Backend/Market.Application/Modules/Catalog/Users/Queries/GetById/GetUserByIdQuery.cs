namespace Market.Application.Modules.Catalog.Users.Queries.GetById;

public sealed class GetUserByIdQuery : IRequest<GetUserByIdQueryDto>
{
    public string PublicId { get; set; } = string.Empty;
}
