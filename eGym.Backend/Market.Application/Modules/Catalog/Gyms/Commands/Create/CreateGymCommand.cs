namespace Market.Application.Modules.Catalog.Gyms.Commands.Create;

public class CreateGymCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
