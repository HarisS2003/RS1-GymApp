namespace Market.Application.Modules.Catalog.Gyms.Queries.GetById;

public class GetGymByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
}
