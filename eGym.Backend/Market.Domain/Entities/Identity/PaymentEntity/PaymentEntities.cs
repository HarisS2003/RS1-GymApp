using Market.Domain.Common;

namespace Market.Domain.Entities;

public class PaymentEntity : BaseEntity
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }

    public UserEntity? User { get; set; }
}
