using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.GraphQL
{
    public class Query
    {
        public async Task<List<CustomerShortResponse>> Customers([Service] IRepository<Customer> customerRepository)
        {
            var customers = await customerRepository.GetAllAsync();

            return customers.Select(x => new CustomerShortResponse
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();
        }

        public async Task<CustomerResponse> Customer(Guid id, [Service] IRepository<Customer> customerRepository)
        {
            var customer = await customerRepository.GetByIdAsync(id);
            return customer == null ? null : new CustomerResponse(customer);
        }
    }
}

