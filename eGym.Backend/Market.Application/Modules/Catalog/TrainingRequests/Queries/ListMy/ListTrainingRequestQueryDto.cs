namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.ListMy;

public sealed class ListTrainingRequestQueryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TrainerId { get; set; }
    public string MemberName { get; set; } = "";
    public string TrainerName { get; set; } = "";
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TrainingRequestStatus Status { get; set; }
}
