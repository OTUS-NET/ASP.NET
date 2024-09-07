using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class PromoCodeCustomersRepository : IPromoCodeCustomersRepository
    {
        private readonly DataContext _dataContext;

        public PromoCodeCustomersRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<PromoCodeCustomer>> GetAllCustomerPromoCodesByCustomerIdAsync(Guid id)
        {
            return await _dataContext.PromoCodeCustomers.Where(x => x.CustomerId == id).ToListAsync();
        }

        public async Task<List<PromoCodeCustomer>> GetAllCustomersByPromoCodeIdAsync(Guid id)
        {
            return await _dataContext.PromoCodeCustomers.Where(x => x.PromoCodeId == id).ToListAsync();
        }
    }
}
