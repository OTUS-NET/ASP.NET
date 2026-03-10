using System;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.GraphQL.Inputs;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.GraphQL
{
    public class Mutation
    {
        public async Task<Guid> CreateCustomer(
            CreateCustomerInput input,
            [Service] IRepository<Customer> customerRepository,
            [Service] IRepository<Preference> preferenceRepository)
        {
            var preferences = await preferenceRepository.GetRangeByIdsAsync(input.PreferenceIds ?? Enumerable.Empty<Guid>().ToList());

            var request = new CreateOrEditCustomerRequest
            {
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName,
                PreferenceIds = input.PreferenceIds ?? new()
            };

            var customer = CustomerMapper.MapFromModel(request, preferences);
            await customerRepository.AddAsync(customer);

            return customer.Id;
        }

        public async Task<bool> EditCustomer(
            EditCustomerInput input,
            [Service] IRepository<Customer> customerRepository,
            [Service] IRepository<Preference> preferenceRepository)
        {
            var customer = await customerRepository.GetByIdAsync(input.Id);
            if (customer == null)
                return false;

            var preferences = await preferenceRepository.GetRangeByIdsAsync(input.PreferenceIds ?? new());

            var request = new CreateOrEditCustomerRequest
            {
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName,
                PreferenceIds = input.PreferenceIds ?? new()
            };

            CustomerMapper.MapFromModel(request, preferences, customer);
            await customerRepository.UpdateAsync(customer);

            return true;
        }

        public async Task<bool> DeleteCustomer(Guid id, [Service] IRepository<Customer> customerRepository)
        {
            var customer = await customerRepository.GetByIdAsync(id);
            if (customer == null)
                return false;

            await customerRepository.DeleteAsync(customer);
            return true;
        }
    }
}

