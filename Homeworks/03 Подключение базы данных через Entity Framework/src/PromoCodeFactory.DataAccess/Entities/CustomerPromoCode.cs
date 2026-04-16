using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Entities;

public class CustomerPromoCode: BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid PromoCodeId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? AppliedAt { get; set; }

    public PromoCode PromoCode { get; set; } = null!;
}