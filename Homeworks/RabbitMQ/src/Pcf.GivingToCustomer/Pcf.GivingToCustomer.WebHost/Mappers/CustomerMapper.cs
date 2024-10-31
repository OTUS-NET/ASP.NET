using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Mappers
{
    public class CustomerMapper
    {

        public static Customer MapFromModel(CreateOrEditCustomerRequest model, IEnumerable<Preference> preferences, Customer customer = null)
        {
            if (customer == null)
            {
                customer = new Customer();
                customer.Id = Guid.NewGuid();
            }

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Email = model.Email;

            customer.Preferences = preferences.Select(x => new CustomerPreference()
            {
                CustomerId = customer.Id,
                Preference = x,
                PreferenceId = x.Id
            }).ToList();

            return customer;
        }
    }
}
