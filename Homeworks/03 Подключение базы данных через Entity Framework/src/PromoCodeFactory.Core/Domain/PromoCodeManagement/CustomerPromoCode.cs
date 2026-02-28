namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class CustomerPromoCode
{
    public Guid CustomerId { get; set; }
    public Guid PromoCodeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AppliedAt { get; set; }
}
