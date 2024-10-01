using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    /// <summary>
    /// This implementation was created in order to add the Include method to the queries.
    /// </summary>
    public class CustomersRepository : EfCoreRepository<Customer>
    {
        private readonly DatabaseContext _dbContext;

        public CustomersRepository(DatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken token)
        {
            return await _dbContext.Customers
                .Include(c => c.CustomersPreferences)
                .ThenInclude(cp => cp.Preference)
                .Include(c => c.PromoCodes)
                .ToListAsync(token);
        }

        public override async Task<Customer> GetByIdAsync(Guid id, CancellationToken token)
        {
            return await _dbContext.Customers
                .Include(c => c.CustomersPreferences)
                .ThenInclude(cp => cp.Preference)
                .Include(c => c.PromoCodes)
                .FirstOrDefaultAsync(c => c.Id == id, token);
        }
        public override async Task CreateAsync(Customer entity, CancellationToken token)
        {
            await _dbContext.Customers.AddAsync(entity, token);
            await _dbContext.SaveChangesAsync(token);

            foreach (var customerPreference in entity.CustomersPreferences) { 
                var preferenceName = await _dbContext.Preferences.Where(p => p.Id == customerPreference.PreferenceId).Select(p => p.Name).SingleOrDefaultAsync(token);
                customerPreference.Preference = new() { Name = preferenceName };
            }

            entity = await _dbContext.Customers.Include(c => c.CustomersPreferences).FirstOrDefaultAsync(e => e.Id == entity.Id, token);
        }

        public override async Task DeleteByIdAsync(Guid id, CancellationToken token)
        {
            // Remove all related PromoCodes
            if (_dbContext.PromoCodes.Where(p => p.CustomerId == id) is IQueryable<PromoCode> promoCodesToDelete)
            {
                _dbContext.PromoCodes.RemoveRange(promoCodesToDelete);
                await _dbContext.SaveChangesAsync(token);
            }

            await base.DeleteByIdAsync(id, token);
        }
    }
}
