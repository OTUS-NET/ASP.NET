using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.Employees;
using PromoCodeFactory.WebHost.Models.Preferences;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class CustomersMappper
{
    public static CustomerShortResponse ToCustomerShortResponse(this Customer customer)
    {
        return new CustomerShortResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(p => new PreferenceShortResponse(
                p.Id,
                p.Name
            )).ToList()
        );
    }

    public static CustomerResponse ToCustomerResponse(this Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,

            customer.Preferences.Select(p => new PreferenceShortResponse(
                p.Id,
                p.Name
            )).ToList(),

            customer.CustomerPromoCodes.Select(c => new CustomerPromoCodeResponse(
                c.Id,
                string.Empty,
                string.Empty,
                string.Empty,
                default,
                default,
                Guid.Empty,
                Guid.Empty,
                c.CreatedAt,
                c.AppliedAt
            )).ToList()
        );
    }

    public static Customer ToCustomer(this CustomerCreateRequest request)
    {
        return new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };
    }

    public static Customer ToCustomer(this CustomerUpdateRequest request, Guid id)
    {
        return new Customer
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            // Создаем заглушки, чтобы передать Id в сервис
            Preferences = request.PreferenceIds
                .Select(pid => new Preference
                {
                    Id = pid,
                    Name = string.Empty
                })
                .ToList()
        };
    }
}
