using Market.Domain.Common;

namespace Market.Domain.Entities;

public class BasketItemEntity : BaseEntity
{
    public int BasketId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public BasketEntity? Basket { get; set; }
    public ProductEntity? Product { get; set; }
}
