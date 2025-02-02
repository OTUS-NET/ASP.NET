using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class PromocodeRepository : EfRepository<PromoCode>
    {
        public PromocodeRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<PromoCode> GetByIdAsync(Guid id)
        {
            return await _data
                .Include(x => x.PartnerManager)
                .Include(x => x.Customer)
                .Include(x => x.Preference)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<IEnumerable<PromoCode>> GetAllAsync(Func<PromoCode, bool> condition = null)
        {
            IEnumerable<PromoCode> query = _data
                .Include(x => x.PartnerManager)
                .Include(x => x.Customer)
                .Include(x => x.Preference);

            if (condition != null)
                query = query.Where(condition);

            return await Task.FromResult(query.ToList());
        }
    }
}
