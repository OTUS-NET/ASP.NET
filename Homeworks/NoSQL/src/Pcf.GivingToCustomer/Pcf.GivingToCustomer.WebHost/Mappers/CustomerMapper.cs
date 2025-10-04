using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pcf.GivingToCustomer.WebHost.Mappers
{
    public class CustomerMapper
    {

        public static Customer MapFromModel(CreateOrEditCustomerRequest model, IEnumerable<Guid> preferenceIds, Customer customer = null)
        {
            if(customer == null)
            {
                customer = new Customer();
                customer.Id = Guid.NewGuid();
            }
            
            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Email = model.Email;

            customer.Preferences = preferenceIds.Select(x => new CustomerPreference()
            {
                CustomerId = customer.Id,
                PreferenceId = x
            }).ToList();
            
            return customer;
        }
    }
}
