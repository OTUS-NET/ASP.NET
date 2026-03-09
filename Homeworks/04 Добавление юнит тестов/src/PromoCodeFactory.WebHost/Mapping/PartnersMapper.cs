using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Partners;

namespace PromoCodeFactory.WebHost.Mapping;

public static class PartnersMapper
{
    public static PartnerResponse ToPartnerResponse(Partner partner)
    {
        return new PartnerResponse(
            partner.Id,
            partner.Name,
            partner.NumberIssuedPromoCodes,
            partner.IsActive,
            partner.Manager.Id,
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
            limit.Limit);
    }
}
