namespace Market.Application.Modules.Catalog.Trainers.Commands.Update;

public sealed class UpdateTrainerCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GymId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}
