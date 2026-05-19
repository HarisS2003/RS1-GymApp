namespace Market.Application.Modules.Catalog.Reviews.Commands.Update;

public sealed class UpdateReviewCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public int Rating { get; set; }
    public string? Comment { get; set; }
}
