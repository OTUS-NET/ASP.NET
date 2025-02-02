using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class CustomerRepository : EfRepository<Customer>
    {
        public CustomerRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            return await _data.Include(x => x.PromoCodes).Include(x => x.Preferences).FirstAsync(x => x.Id == id);
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync(Func<Customer, bool> condition = null)
        {
            IEnumerable<Customer> query = _data.Include(x => x.PromoCodes).Include(x => x.Preferences);
            if (condition != null)
                query = query.Where(condition);

            return await Task.FromResult(query.ToList());
        }
    }
}
