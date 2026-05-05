using Market.Domain.Common;

namespace Market.Domain.Entities;

public class TrainingRequestEntity : BaseEntity
{
    public int UserId { get; set; }
    public int TrainerId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TrainingRequestStatus Status { get; set; }

    public UserEntity? User { get; set; }
    public TrainerEntity? Trainer { get; set; }
}
