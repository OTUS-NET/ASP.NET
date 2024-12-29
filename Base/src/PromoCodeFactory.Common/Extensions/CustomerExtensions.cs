using PromoCodeFactory.Contracts.Customers;
using PromoCodeFactory.Contracts.Preferences;
using PromoCodeFactory.Core.PromoCodeManagement;

namespace PromoCodeFactory.Common.Extensions;

public static class CustomerExtensions
{
    public static CustomerResponseDto MapToCustomerResponseDto(this Customer customer)
    {
        return new CustomerResponseDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Preferences = customer.CustomerPreferences?.Select(cp => new PreferenceResponseDto
            {
                Id = cp.Preference.Id,
                Name = cp.Preference.Name
            }).ToList() ?? []
        };
    }
}