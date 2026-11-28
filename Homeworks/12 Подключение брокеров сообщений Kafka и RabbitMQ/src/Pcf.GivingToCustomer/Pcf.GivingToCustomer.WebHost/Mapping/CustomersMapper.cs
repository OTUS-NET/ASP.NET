using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models.Customers;
using Pcf.GivingToCustomer.WebHost.Models.Preferences;
using Pcf.GivingToCustomer.WebHost.Models.PromoCodes;

namespace Pcf.GivingToCustomer.WebHost.Mapping;

public static class CustomersMapper
{
    public static CustomerShortResponse ToCustomerShortResponse(
        Customer customer,
        IReadOnlyDictionary<Guid, Preference> preferences)
    {
        return new CustomerShortResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            ToPreferenceResponses(customer.PreferenceIds, preferences));
    }

    public static CustomerResponse ToCustomerResponse(
        Customer customer,
        IReadOnlyDictionary<Guid, Preference> preferences,
        IReadOnlyCollection<PromoCode> promoCodes)
    {
        var promoCodeResponses = promoCodes.Select(p =>
        {
            var customerPromoCode = customer.CustomerPromoCodes.Single(e => e.PromoCodeId == p.Id);
            return new CustomerPromoCodeResponse(
                    p.Id,
                    p.Code,
                    p.ServiceInfo,
                    p.PartnerId,
                    p.BeginDate,
                    p.EndDate,
                    p.PreferenceId,
                    customerPromoCode.CreatedAt,
                    customerPromoCode.AppliedAt);
        }).ToList();

        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            ToPreferenceResponses(customer.PreferenceIds, preferences),
            promoCodeResponses);
    }

    private static List<PreferenceShortResponse> ToPreferenceResponses(
        IEnumerable<Guid> preferenceIds,
        IReadOnlyDictionary<Guid, Preference> preferences)
    {
        return preferenceIds
            .Where(preferences.ContainsKey)
            .Select(id => new PreferenceShortResponse(id, preferences[id].Name))
            .ToList();
    }
}
