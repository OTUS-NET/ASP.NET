using Pcf.Common.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Domain;

public class Partner : BaseEntity
{
    public required string Name { get; set; }

    public required bool IsActive { get; set; }

    public required Guid ManagerId { get; set; }

    public ICollection<PartnerPromoCodeLimit> PartnerLimits { get; set; } = [];
}
