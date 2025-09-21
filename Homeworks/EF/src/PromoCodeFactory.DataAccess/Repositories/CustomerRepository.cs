using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class CustomerRepository : EfRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(DataContext dataContext) : base (dataContext)
        {
            
        }

        /// <summary>
        /// Для получения данных о Customer с дополнительными данными по Preference и PromoCode
        /// </summary>
        public async Task<Customer> GetCustomerWithAllProperties(Guid id)
        {
            return await _context.Customers
                .Include(x => x.PromoCodes)
                .Include(x => x.CustomerPreferences)
                .ThenInclude(x => x.Preference)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Для получения данных о Customer с дополнительными данными по Preference и PromoCode
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomersWithAllProperties()
        {
            return await _context.Customers
                .Include(x => x.PromoCodes)
                .Include(x => x.CustomerPreferences)
                .ThenInclude(x => x.Preference)
                .ToListAsync();
        }

        public async Task<Customer> UpdateCustomerAsync(Customer item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            var preferences = await _context.CustomerPreferences.Where(x => x.CustomerId == item.Id).ToListAsync();
            if (preferences is not null && preferences.Any())
            {
                _context.CustomerPreferences.RemoveRange(preferences);
                await _context.SaveChangesAsync();
            }
            _context.Customers.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
