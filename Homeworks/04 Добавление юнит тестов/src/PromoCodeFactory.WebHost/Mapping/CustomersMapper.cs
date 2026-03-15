using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.Preferences;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class CustomersMapper
{
    public static CustomerShortResponse ToCustomerShortResponse(Customer customer)
    {
        var preferenceResponses = customer.Preferences
            .Select(p => new PreferenceShortResponse(p.Id, p.Name))
            .ToList();

        return new CustomerShortResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            preferenceResponses);
    }

    public static CustomerResponse ToCustomerResponse(Customer customer, IReadOnlyCollection<PromoCode> promoCodes)
    {
        var preferenceResponses = customer.Preferences
            .Select(p => new PreferenceShortResponse(p.Id, p.Name))
            .ToList();

        var promoCodeResponses = promoCodes.Select(p =>
        {
            var customerPromoCode = customer.CustomerPromoCodes.Single(e => e.PromoCodeId == p.Id);
            return new CustomerPromoCodeResponse(
                    p.Id,
                    p.Code,
                    p.ServiceInfo,
                    p.Partner.Id,
                    p.BeginDate,
                    p.EndDate,
                    p.Preference.Id,
                    customerPromoCode.CreatedAt,
                    customerPromoCode.AppliedAt);
        }).ToList();

        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            preferenceResponses,
            promoCodeResponses);
    }
}
