using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class CustomerPreferenceRepository : ICustomerPreferenceRepository
    {
        private readonly DataContext _dataContext;

        public CustomerPreferenceRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<CustomerPreference>> GetAllCustomerPreferencesByCustomerIdAsync(Guid id)
        {
            return await _dataContext.CustomerPreferences.Where(x => x.CustomerId == id).ToListAsync();
        }

        public async Task<List<CustomerPreference>> GetAllCustomersByPreferenceIdAsync(Guid id)
        {
            return await _dataContext.CustomerPreferences.Where(x => x.PreferenceId == id).ToListAsync();
        }
    }
}
