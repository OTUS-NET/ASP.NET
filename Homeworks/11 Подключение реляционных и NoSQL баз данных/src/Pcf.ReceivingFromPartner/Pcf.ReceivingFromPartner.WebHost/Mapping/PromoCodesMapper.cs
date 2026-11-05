using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Models.PromoCodes;

namespace Pcf.ReceivingFromPartner.WebHost.Mapping;

public static class PromoCodesMapper
{
    public static PromoCodeShortResponse ToPromoCodeShortResponse(PromoCode promoCode)
    {
        return new PromoCodeShortResponse(
            promoCode.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.Partner.Id,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.Preference.Id);
    }
}
