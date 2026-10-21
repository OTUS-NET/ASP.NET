using Pcf.Common.Core.Domain;

namespace Pcf.GivingToCustomer.Core.Domain;

public class PromoCode : BaseEntity
{
    public required string Code { get; set; }

    public required string ServiceInfo { get; set; }

    public required DateTimeOffset BeginDate { get; set; }

    public required DateTimeOffset EndDate { get; set; }

    public required Guid PartnerId { get; set; }

    public required Preference Preference { get; set; }

    public ICollection<CustomerPromoCode> CustomerPromoCodes { get; set; } = [];
}
