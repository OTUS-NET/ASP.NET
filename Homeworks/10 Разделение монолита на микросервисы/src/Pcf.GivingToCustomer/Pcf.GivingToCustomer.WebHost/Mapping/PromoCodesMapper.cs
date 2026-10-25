using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models.PromoCodes;

namespace Pcf.GivingToCustomer.WebHost.Mapping;

public static class PromoCodesMapper
{
    public static PromoCodeShortResponse ToPromoCodeShortResponse(PromoCode promoCode)
    {
        return new PromoCodeShortResponse(
            promoCode.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.PartnerId,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.Preference.Id);
    }
}
