using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class CustomerMapper
{
    public static CustomerShortResponse ToCustomerShortResponse(Customer customer)
    {
        return new CustomerShortResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToArray());
    }

    public static CustomerResponse ToCustomerResponse(
        Customer customer,
        IEnumerable<(CustomerPromoCode customerPromoCode, PromoCode promoCode)> tuples)
    {
        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToArray(),
            tuples.Select(t => CustomerPromoCodesMapper.ToCustomerPromoCodeResponse(t.customerPromoCode, t.promoCode)).ToArray()
            );
    }

    public static Customer ToCustomer(CustomerUpdateRequest request, IEnumerable<Preference> preferences)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Preferences = preferences.ToArray(),
        };
    }

    public static Customer ToCustomer(CustomerCreateRequest request, IEnumerable<Preference> preferences)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Preferences = preferences.ToArray(),
        };
    }
}
