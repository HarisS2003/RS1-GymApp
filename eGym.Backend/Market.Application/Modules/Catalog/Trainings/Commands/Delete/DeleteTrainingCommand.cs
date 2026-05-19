namespace Market.Application.Modules.Catalog.Trainings.Commands.Delete;

public sealed class DeleteTrainingCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
