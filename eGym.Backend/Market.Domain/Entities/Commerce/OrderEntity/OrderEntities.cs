using Market.Domain.Common;

namespace Market.Domain.Entities;

public class OrderEntity : BaseEntity
{
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;

    public UserEntity? User { get; set; }
    public ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
}
