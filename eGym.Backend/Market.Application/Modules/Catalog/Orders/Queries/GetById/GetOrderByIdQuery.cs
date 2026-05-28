namespace Market.Application.Modules.Catalog.Orders.Queries.GetById;

public sealed class GetOrderByIdQuery : IRequest<GetOrderByIdQueryDto>
{
    public required int Id { get; init; }
}
