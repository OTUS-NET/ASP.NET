using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

/// <summary>
/// Методы преобразования доменных моделей промокодов и выдач промокодов в DTO API.
/// </summary>
public static class PromoCodesMapper
{
    /// <summary>
    /// ToPromoCodeShortResponse - преобразует доменный промокод в краткую модель ответа.
    /// В ответе используются идентификаторы связанных сущностей: менеджера-партнера и предпочтения.
    /// </summary>
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

    /// <summary>
    /// ToCustomerPromoCodeResponse - преобразует запись выдачи промокода клиенту в DTO.
    /// Объединяет технические данные выдачи с данными самого промокода.
    /// </summary>
    public static CustomerPromoCodeResponse ToCustomerPromoCodeResponse(CustomerPromoCode customerPromoCode, PromoCode promoCode)
    {
        return new CustomerPromoCodeResponse(
            customerPromoCode.Id,
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

    /// <summary>
    /// ToPromoCode - создает доменную модель промокода из запроса API.
    /// Связанные Employee и Preference должны быть заранее загружены и проверены контроллером.
    /// </summary>
    public static PromoCode ToPromoCode(PromoCodeCreateRequest request, Employee partnerManager, Preference preference)
    {
        return new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            ServiceInfo = request.ServiceInfo,
            PartnerName = request.PartnerName,
            BeginDate = request.BeginDate,
            EndDate = request.EndDate,
            PartnerManager = partnerManager,
            Preference = preference
        };
    }
}
