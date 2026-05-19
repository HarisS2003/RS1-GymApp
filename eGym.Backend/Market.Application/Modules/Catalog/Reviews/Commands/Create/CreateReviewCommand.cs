namespace Market.Application.Modules.Catalog.Reviews.Commands.Create;

public sealed class CreateReviewCommand : IRequest<int>
{
    public int UserId { get; set; }
    public int TrainerId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
