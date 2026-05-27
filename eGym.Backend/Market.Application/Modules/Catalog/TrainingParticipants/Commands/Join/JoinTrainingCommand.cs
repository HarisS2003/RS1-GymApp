namespace Market.Application.Modules.Catalog.TrainingParticipants.Commands.Join;

public sealed class JoinTrainingCommand : IRequest<Unit>
{
    public int TrainingId { get; set; }
}
