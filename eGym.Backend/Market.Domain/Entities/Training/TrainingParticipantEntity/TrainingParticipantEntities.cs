using Market.Domain.Common;

namespace Market.Domain.Entities;

public class TrainingParticipantEntity : BaseEntity
{
    public int TrainingId { get; set; }
    public int UserId { get; set; }

    public TrainingEntity? Training { get; set; }
    public UserEntity? User { get; set; }
}
