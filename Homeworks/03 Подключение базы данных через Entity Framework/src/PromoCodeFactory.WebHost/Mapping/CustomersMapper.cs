using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;

namespace PromoCodeFactory.WebHost.Mapping;

/// <summary>
/// Методы преобразования доменной модели клиента в DTO API и обратно.
/// </summary>
public static class CustomersMapper
{
    /// <summary>
    /// ToCustomerShortResponse - преобразует клиента в краткую модель ответа для списков и операций создания/обновления.
    /// В ответ включаются основные поля клиента и список его предпочтений.
    /// </summary>
    public static CustomerShortResponse ToCustomerShortResponse(Customer customer)
    {
        return new CustomerShortResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToList());
    }

    /// <summary>
    /// ToCustomerResponse - преобразует клиента в полную модель ответа.
    /// Доп. сопоставляет записи выдачи CustomerPromoCode с самими промокодами,
    /// чтобы вернуть клиенту не только факт выдачи, но и данные промокода.
    /// </summary>
    public static CustomerResponse ToCustomerResponse(Customer customer, IReadOnlyCollection<PromoCode> promoCodes)
    {
        // Используем соварь чтобы быстро найти доменный PromoCode по PromoCodeId из записи CustomerPromoCode.
        var promoCodesById = promoCodes.ToDictionary(promoCode => promoCode.Id);
        var customerPromoCodes = customer.CustomerPromoCodes
            .Where(customerPromoCode => promoCodesById.ContainsKey(customerPromoCode.PromoCodeId))
            .Select(customerPromoCode => PromoCodesMapper.ToCustomerPromoCodeResponse(
                customerPromoCode,
                promoCodesById[customerPromoCode.PromoCodeId]))
            .ToList();

        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToList(),
            customerPromoCodes);
    }

    /// <summary>
    /// ToCustomer - создает доменную модель клиента из запроса на создание.
    /// Предпочтения передаются уже загруженными из базы, чтобы EF Core корректно создал связи many-to-many.
    /// </summary>
    public static Customer ToCustomer(CustomerCreateRequest request, IReadOnlyCollection<Preference> preferences)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Preferences = preferences.ToList()
        };
    }
}
