using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class CustomerRepository : EfRepository<Customer>
    {
        public CustomerRepository(DataContext dataContext) : base(dataContext) { }

        public override async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Customer>()
                                   .Include(c => c.CustomerPreferences)
                                   .ToListAsync();
        }

        public override async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _entitySet.Include(c => c.PromoCodes)
                                   .Include(c => c.CustomerPreferences)
                                   .ThenInclude(p => p.Preference)
                                   .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<Customer> UpdateAsync(Customer obj, CancellationToken cancellationToken = default)
        {
            var customer = await GetByIdAsync(obj.Id, cancellationToken);

            if (customer == null)
                return null;

            _entitySet.Entry(customer).CurrentValues.SetValues(obj);

            customer.CustomerPreferences = obj.CustomerPreferences;

            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return customer;
        }

        public override async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var customer = await GetByIdAsync(id, cancellationToken);

            if (customer == null)
                return false;

             _dbContext.RemoveRange(customer.PromoCodes);
             _dbContext.Remove(customer);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}