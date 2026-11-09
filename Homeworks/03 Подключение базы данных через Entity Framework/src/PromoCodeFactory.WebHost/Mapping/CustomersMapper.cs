using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class CustomersMapper
{
    /// <summary>
    /// Преобразует доменную модель <see cref="Customer"/> в DTO <see cref="CustomerShortResponse"/>.
    /// </summary>
    /// <param name="customer">Экземпляр <see cref="Customer"/>, который нужно преобразовать.</param>
    /// <returns>Объект <see cref="CustomerShortResponse"/>, содержащий данные, преобразованные из указанного <see cref="Customer"/>.</returns>
    public static CustomerShortResponse ToCustomerShortResponse(Customer customer)
    {
        return new CustomerShortResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            [
                .. customer.Preferences
                    .Select(PreferencesMapper.ToPreferenceShortResponse)
            ]);
    }

    /// <summary>
    /// Преобразует доменную модель <see cref="Customer"/> в DTO <see cref="CustomerResponse"/>.
    /// </summary>
    /// <param name="customer">Экземпляр <see cref="Customer"/>, который необходимо преобразовать.</param>
    /// <param name="promoCodesById">
    /// Словарь, содержащий экземпляры <see cref="PromoCode"/>, индексированные по их идентификаторам.
    /// Используется для сопоставления промокодов, связанных с клиентом.
    /// </param>
    /// <returns>
    /// Объект <see cref="CustomerResponse"/>, содержащий данные, преобразованные из указанного <see cref="Customer"/> 
    /// и его связанных промокодов.
    /// </returns>
    public static CustomerResponse ToCustomerResponse(
        Customer customer,
        IReadOnlyDictionary<Guid, PromoCode> promoCodesById)
    {
        var promoCodeResponses = customer.CustomerPromoCodes
            .Where(cpc => promoCodesById.ContainsKey(cpc.PromoCodeId))
            .Select(cpc =>
            {
                var promoCode = promoCodesById[cpc.PromoCodeId];
                return new CustomerPromoCodeResponse(
                    cpc.Id,
                    promoCode.Code,
                    promoCode.ServiceInfo,
                    promoCode.PartnerName,
                    promoCode.BeginDate,
                    promoCode.EndDate,
                    promoCode.PartnerManager.Id,
                    promoCode.Preference.Id,
                    cpc.CreatedAt,
                    cpc.AppliedAt);
            })
            .ToList();

        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            [
                .. customer.Preferences
                    .Select(PreferencesMapper.ToPreferenceShortResponse)
            ],
            promoCodeResponses);
    }
}
