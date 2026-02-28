using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class PromoCode : BaseEntity
{
    public required string Code { get; set; }

    public required string ServiceInfo { get; set; }

    public required DateTime BeginDate { get; set; }

    public required DateTime EndDate { get; set; }

    public required string PartnerName { get; set; }

    public required Employee PartnerManager { get; set; }

    public required Preference Preference { get; set; }
}
