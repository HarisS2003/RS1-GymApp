using Market.Application.Modules.Catalog.Baskets.Queries.GetById;

namespace Market.Application.Modules.Catalog.Baskets.Queries.GetByUserId;

public sealed class GetBasketByUserIdQuery : IRequest<GetBasketByIdQueryDto>
{
    public int UserId { get; set; }
}
