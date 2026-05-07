namespace Market.Application.Modules.Catalog.Baskets.Queries.GetById;

public sealed class GetBasketByIdQuery : IRequest<GetBasketByIdQueryDto>
{
    public int Id { get; set; }
}
