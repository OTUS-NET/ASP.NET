using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class CustomersMapper
{
    public static CustomerShortResponse ToCustomerShortResponse(Customer customer)
    {
        return new CustomerShortResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToList());
    }

    public static CustomerResponse ToCustomerResponse(Customer customer, IReadOnlyCollection<PromoCode> promoCodes)
    {
        var promoCodesById = promoCodes.ToDictionary(p => p.Id);

        var promoCodeResponses = customer.CustomerPromoCodes
            .Select(cpc => ToCustomerPromoCodeResponse(cpc, promoCodesById[cpc.PromoCodeId]))
            .ToList();

        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToList(),
            promoCodeResponses);
    }

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

    private static CustomerPromoCodeResponse ToCustomerPromoCodeResponse(CustomerPromoCode customerPromoCode, PromoCode promoCode)
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
}
