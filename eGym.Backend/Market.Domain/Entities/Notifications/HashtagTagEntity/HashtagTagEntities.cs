using Market.Domain.Common;

namespace Market.Domain.Entities;

public class HashtagTagEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
}
