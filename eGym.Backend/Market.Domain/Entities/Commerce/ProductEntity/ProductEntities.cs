using Market.Domain.Common;

namespace Market.Domain.Entities;

public class ProductEntity : BaseEntity
{
    public int GymId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public GymEntity? Gym { get; set; }
    public ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    public ICollection<BasketItemEntity> BasketItems { get; set; } = new List<BasketItemEntity>();
}
