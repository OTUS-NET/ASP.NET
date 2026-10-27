using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class PromoCodesMapper
{
    public static PromoCodeShortResponse ToPromoCodeShortResponse(this PromoCode promoCode)
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

    public static PromoCode ToPromoCode(this PromoCodeCreateRequest request)
    {
        return new PromoCode
        {
            Code = request.Code,
            ServiceInfo = request.ServiceInfo,
            PartnerName = request.PartnerName,
            BeginDate = request.BeginDate,
            EndDate = request.EndDate,

            PartnerManager = new Employee
            {
                Id = request.PartnerManagerId,
                FirstName = string.Empty,
                LastName = string.Empty,
                Email = string.Empty,
                Role = null!
            },
            Preference = new Preference { Id = request.PreferenceId, Name = string.Empty }
        };
    }
}
