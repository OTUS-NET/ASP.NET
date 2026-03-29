using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class Partner : BaseEntity
{
    public required string Name { get; set; }

    public required bool IsActive { get; set; }

    public required Employee Manager { get; set; }

    public ICollection<PartnerPromoCodeLimit> PartnerLimits { get; set; } = [];
}
