using Market.Domain.Common;

namespace Market.Domain.Entities;

public class BasketEntity : BaseEntity
{
    public int UserId { get; set; }

    public UserEntity? User { get; set; }
    public ICollection<BasketItemEntity> BasketItems { get; set; } = new List<BasketItemEntity>();
}
