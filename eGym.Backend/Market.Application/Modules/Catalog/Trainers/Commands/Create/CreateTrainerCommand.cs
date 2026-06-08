namespace Market.Application.Modules.Catalog.Trainers.Commands.Create;

public sealed class CreateTrainerCommand : IRequest<string>
{
    public string UserPublicId { get; set; } = string.Empty;
    public int GymId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}
