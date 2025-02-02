using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost.Extensions
{
    public static class CustomerExtensions
    {
        /// <summary>
        /// Конвертирует Customer в CustomerShortResponse
        /// </summary>
        public static CustomerShortResponse ToShortResponse(this Customer customer)
        {
            return new CustomerShortResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            };
        }

        /// <summary>
        /// Конвертирует Customer в CustomerResponse
        /// </summary>
        public static CustomerResponse ToResponse(this Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = customer.Preferences.Select(p => p.ToResponse()).ToList(),
                PromoCodes = customer.PromoCodes.Select(pc => pc.ToShortResponse()).ToList()
            };
        }

        /// <summary>
        /// Конвертирует CreateOrEditCustomerRequest в Customer
        /// </summary>
        public static Customer ToCustomer(this CreateOrEditCustomerRequest request, IEnumerable<Preference> preferences)
        {
            return new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Preferences = preferences.Where(p => request.PreferenceIds.Contains(p.Id)).ToList()
            };
        }
    }
}
