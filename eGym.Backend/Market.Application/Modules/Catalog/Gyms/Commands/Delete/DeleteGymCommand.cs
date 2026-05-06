namespace Market.Application.Modules.Catalog.Gyms.Commands.Delete;

public class DeleteGymCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
