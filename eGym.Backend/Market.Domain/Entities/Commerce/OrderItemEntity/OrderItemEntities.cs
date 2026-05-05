using Market.Domain.Common;

namespace Market.Domain.Entities;

public class OrderItemEntity : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }

    public OrderEntity? Order { get; set; }
    public ProductEntity? Product { get; set; }
}
