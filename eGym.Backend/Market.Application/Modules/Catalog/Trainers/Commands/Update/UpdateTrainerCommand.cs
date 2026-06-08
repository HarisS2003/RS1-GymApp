namespace Market.Application.Modules.Catalog.Trainers.Commands.Update;

public sealed class UpdateTrainerCommand : IRequest<Unit>
{
    public string PublicId { get; set; } = string.Empty;
    public string UserPublicId { get; set; } = string.Empty;
    public int GymId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}
