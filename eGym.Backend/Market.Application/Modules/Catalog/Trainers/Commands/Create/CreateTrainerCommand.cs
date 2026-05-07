namespace Market.Application.Modules.Catalog.Trainers.Commands.Create;

public sealed class CreateTrainerCommand : IRequest<int>
{
    public int UserId { get; set; }
    public int GymId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}
