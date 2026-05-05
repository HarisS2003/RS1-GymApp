using Market.Domain.Common;

namespace Market.Domain.Entities;

public class TrainerEntity : BaseEntity
{
    public int UserId { get; set; }
    public int GymId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }

    public UserEntity? User { get; set; }
    public GymEntity? Gym { get; set; }
    public ICollection<TrainingEntity> Trainings { get; set; } = new List<TrainingEntity>();
    public ICollection<TrainingRequestEntity> TrainingRequests { get; set; } = new List<TrainingRequestEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
}
