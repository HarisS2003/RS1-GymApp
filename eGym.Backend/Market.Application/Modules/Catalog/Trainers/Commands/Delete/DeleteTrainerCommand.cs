namespace Market.Application.Modules.Catalog.Trainers.Commands.Delete;

public sealed class DeleteTrainerCommand : IRequest<Unit>
{
    public required string PublicId { get; set; }
}
