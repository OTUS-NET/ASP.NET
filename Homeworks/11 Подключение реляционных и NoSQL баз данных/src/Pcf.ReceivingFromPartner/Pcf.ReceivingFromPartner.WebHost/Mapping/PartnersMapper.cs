using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Models.Partners;

namespace Pcf.ReceivingFromPartner.WebHost.Mapping;

public static class PartnersMapper
{
    public static PartnerResponse ToPartnerResponse(Partner partner)
    {
        return new PartnerResponse(
            partner.Id,
            partner.Name,
            partner.IsActive,
            partner.ManagerId,
            partner.PartnerLimits
                .OrderByDescending(l => l.CreatedAt)
                .Select(ToPartnerPromoCodeLimitResponse)
                .ToList());
    }

    public static PartnerPromoCodeLimitResponse ToPartnerPromoCodeLimitResponse(PartnerPromoCodeLimit limit)
    {
        return new PartnerPromoCodeLimitResponse(
            limit.Id,
            limit.CreatedAt,
            limit.EndAt,
            limit.CanceledAt,
            limit.Limit,
            limit.IssuedCount);
    }
}
