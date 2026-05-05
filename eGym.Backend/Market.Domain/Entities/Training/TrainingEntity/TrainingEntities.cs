using Market.Domain.Common;

namespace Market.Domain.Entities;

public class TrainingEntity : BaseEntity
{
    public int TrainerId { get; set; }
    public TrainingType Type { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public int Capacity { get; set; }

    public TrainerEntity? Trainer { get; set; }
    public ICollection<TrainingParticipantEntity> TrainingParticipants { get; set; } = new List<TrainingParticipantEntity>();
}
