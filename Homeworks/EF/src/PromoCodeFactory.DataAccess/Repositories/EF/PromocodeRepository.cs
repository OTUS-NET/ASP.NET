using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories.EF
{
    public class PromocodeRepository : EfRepositoryBase<PromoCode>
    {
        public PromocodeRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<PromoCode> GetByIdAsync(Guid id)
        {
            return await _entitySet
                .Include(x => x.PartnerManager)
                .Include(x => x.Customer)
                .Include(x => x.Preference)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<IEnumerable<PromoCode>> GetAllAsync(Func<PromoCode, bool> condition = null)
        {
            //TODO: аргумент - флаг, обозначающий, нужно ли подгружать внутренние таблицы
            IEnumerable<PromoCode> query = _entitySet
                .Include(x => x.PartnerManager)
                .Include(x => x.Customer)
                .Include(x => x.Preference);

            if (condition != null)
                query = query.Where(condition);

            return await Task.FromResult(query.ToList());
        }
    }
}
