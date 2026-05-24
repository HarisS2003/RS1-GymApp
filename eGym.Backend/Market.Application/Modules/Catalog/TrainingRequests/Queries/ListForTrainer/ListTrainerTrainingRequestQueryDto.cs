namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.ListForTrainer;

public sealed class ListTrainerTrainingRequestQueryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TrainerId { get; set; }
    public string MemberName { get; set; } = "";
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TrainingRequestStatus Status { get; set; }
}
