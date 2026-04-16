using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class PromoCodesMapper
{
    public static PromoCodeShortResponse ToPromoCodeShortResponse(PromoCode promoCode)
    {
        return new PromoCodeShortResponse(
            promoCode.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.PartnerName,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.PartnerManager.Id,
            promoCode.Preference.Id);
    }

    public static PromoCode ToPromoCode(PromoCodeCreateRequest request, Preference preference, Employee partnerManager)
    {
        return new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            ServiceInfo = request.ServiceInfo,
            PartnerName = request.PartnerName,
            BeginDate = request.BeginDate,
            EndDate = request.EndDate,
            Preference = preference,
            PartnerManager = partnerManager
        };
    }

    public static CustomerPromoCodeResponse ToCustomerPromoCodeResponse(CustomerPromoCode customerPromoCode)
    {
        return new CustomerPromoCodeResponse(
            customerPromoCode.Id,
            customerPromoCode.PromoCode.Code,
            customerPromoCode.PromoCode.ServiceInfo,
            customerPromoCode.PromoCode.PartnerName,
            customerPromoCode.PromoCode.BeginDate,
            customerPromoCode.PromoCode.EndDate,
            customerPromoCode.PromoCode.PartnerManager.Id,
            customerPromoCode.PromoCode.Preference.Id,
            customerPromoCode.CreatedAt,
            customerPromoCode.AppliedAt);
    }
}
