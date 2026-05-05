using Market.Domain.Common;

namespace Market.Domain.Entities;

public class ReviewEntity : BaseEntity
{
    public int UserId { get; set; }
    public int TrainerId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }

    public UserEntity? User { get; set; }
    public TrainerEntity? Trainer { get; set; }
}
