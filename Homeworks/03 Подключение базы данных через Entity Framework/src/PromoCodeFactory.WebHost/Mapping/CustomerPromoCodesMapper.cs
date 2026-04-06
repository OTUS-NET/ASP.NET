using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class CustomerPromoCodesMapper
{
    public static CustomerPromoCodeResponse ToCustomerPromoCodeResponse(PromoCode promoCode, CustomerPromoCode link)
    {
        return new CustomerPromoCodeResponse(
            link.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.PartnerName,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.PartnerManager.Id,
            promoCode.Preference.Id,
            link.CreatedAt,
            link.AppliedAt);
    }
}
