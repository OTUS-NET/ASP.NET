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

    public static CustomerPromoCodeResponse ToCustomerPromoCodeResponse(CustomerPromoCode customerPromoCode, PromoCode promoCode)
    {
        return new CustomerPromoCodeResponse(
            promoCode.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.PartnerName,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.PartnerManager.Id,
            promoCode.Preference.Id,
            customerPromoCode.CreatedAt,
            customerPromoCode.AppliedAt);
    }

    public static PromoCode ToPromoCode(PromoCodeCreateRequest request, Employee partnerManager,
        Preference preference, IEnumerable<CustomerPromoCode> customerPromoCodes)
    {
        return new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            BeginDate = request.BeginDate,
            EndDate = request.EndDate,
            PartnerName = request.PartnerName,
            ServiceInfo = request.ServiceInfo,
            PartnerManager = partnerManager,
            Preference = preference,
            CustomerPromoCodes = customerPromoCodes.ToList()
        };
    }
}
