using Pcf.Common.Core.Domain;

namespace Pcf.GivingToCustomer.Core.Domain;

public class CustomerPromoCode : BaseEntity
{
    public Guid CustomerId { get; set; }

    public Guid PromoCodeId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? AppliedAt { get; set; }
}
