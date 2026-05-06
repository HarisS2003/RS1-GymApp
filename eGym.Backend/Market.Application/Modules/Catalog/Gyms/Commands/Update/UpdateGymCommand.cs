namespace Market.Application.Modules.Catalog.Gyms.Commands.Update;

public sealed class UpdateGymCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
