using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost.Utils
{
    public static class Extensions
    {
        public static List<CustomerShortResponse> ToShortResponseList(this IEnumerable<Customer> customers)
        {
            List<CustomerShortResponse> csr = new();
            foreach (var customer in customers)
            {
                csr.Add(customer.ToShortResponse());
            }

            return csr;
        }

        public static List<CustomerResponse> ToResponseList(this IEnumerable<Customer> customers)
        {
            List<CustomerResponse> csr = new();
            foreach (var customer in customers)
            {
                csr.Add(customer.ToResponse());
            }

            return csr;
        }

        public static CustomerShortResponse ToShortResponse(this Customer customer) =>
            new()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            };

        public static CustomerResponse ToResponse(this Customer customer) =>
            new()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = customer.CustomersPreferences.Select(x => new PreferenceResponse
                    { Id = x.PreferenceId, Name = x.Preference.Name }).ToList(),
                PromoCodes = customer.PromoCodes is not null ? customer.PromoCodes.ToShortResponseList() : null
            };

        public static Customer ToCustomer(this CreateOrEditCustomerRequest request, Guid? id = null)
        {
            var customerId = id ?? Guid.NewGuid();
            var customer = new Customer
            {
                Id = customerId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomersPreferences = request.PreferenceIds.Count > 0 ? new List<CustomerPreference>() : null
            };

            if (customer.CustomersPreferences is not null)
            {
                foreach (var guid in request.PreferenceIds)
                {
                    customer.CustomersPreferences.Add(new CustomerPreference { CustomerId = customerId, PreferenceId = guid });
                }
            }

            return customer;
        }

        public static IEnumerable<PreferenceResponse> ToResponseList(this IEnumerable<Preference> preferences) =>
            preferences.Select(x =>
                new PreferenceResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();

        public static List<PromoCodeShortResponse> ToShortResponseList(this IEnumerable<PromoCode> promoCodes) =>
            promoCodes.Select(x => new PromoCodeShortResponse
            {
                Id = x.Id,
                Code = x.Code,
                ServiceInfo = x.ServiceInfo,
                PartnerName = x.PartnerName,
                BeginDate = x.BeginDate.ToShortDateString(),
                EndDate = x.EndDate.ToShortDateString(),
            }).ToList();
    }
}
