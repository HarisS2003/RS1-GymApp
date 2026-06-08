using Market.Domain.Common;

namespace Market.Domain.Entities;

public class NotificationEntity : BaseEntity
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? HashtagTagId { get; set; }
    public bool IsRead { get; set; }

    public UserEntity? User { get; set; }
    public HashtagTagEntity? HashtagTag { get; set; }
}
