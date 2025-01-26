using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories.EF
{
    public class CustomerRepository : EfRepositoryBase<Customer>
    {
        public CustomerRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            return await _entitySet.Include(x => x.Promocodes).Include(x => x.Preferences).FirstAsync(x => x.Id == id);
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync(Func<Customer, bool> condition = null)
        {
            //TODO: аргумент - флаг, обозначающий, нужно ли подгружать внутренние таблицы
            IEnumerable<Customer> query = _entitySet.Include(x => x.Promocodes).Include(x => x.Preferences);
            if (condition != null)
                query = query.Where(condition);

            return await Task.FromResult(query.ToList());
        }


    }
}
