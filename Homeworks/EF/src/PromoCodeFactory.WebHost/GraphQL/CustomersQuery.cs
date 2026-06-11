using HotChocolate;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PromoCodeFactory.WebHost.GraphQL;

public class CustomersQuery
{
    public async Task<List<CustomerShortResponse>> GetCustomers(
        [Service] IRepository<Customer> customerRepository)
    {
        var customers = await customerRepository.GetAllAsync();

        return customers.Select(customer => new CustomerShortResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email
        }).ToList();
    }

    public async Task<CustomerResponse> GetCustomer(
        Guid id,
        [Service] IRepository<Customer> customerRepository)
    {
        var customer = await customerRepository.GetByIdAsync(id);

        if (customer == null)
        {
            return null;
        }

        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Preferences = customer.CustomerPreferences
                .Select(customerPreference => new PreferenceResponse
                {
                    Id = customerPreference.Preference.Id,
                    Name = customerPreference.Preference.Name
                })
                .ToList(),
            PromoCodes = customer.PromoCodes
                .Select(promoCode => new PromoCodeShortResponse
                {
                    Id = promoCode.Id,
                    Code = promoCode.Code,
                    ServiceInfo = promoCode.ServiceInfo,
                    BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
                    PartnerName = promoCode.PartnerName
                })
                .ToList()
        };
    }
}