using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Repositories
{
    public interface ICustomerPreferenceRepository
    {
        Task<List<CustomerPreference>> GetAllCustomerPreferencesByCustomerIdAsync(Guid id);
        Task<List<CustomerPreference>> GetAllCustomersByPreferenceIdAsync(Guid id);
    }
}
