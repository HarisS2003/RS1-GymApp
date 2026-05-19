namespace Market.Application.Modules.Catalog.Reviews.Commands.Delete;

public sealed class DeleteReviewCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
