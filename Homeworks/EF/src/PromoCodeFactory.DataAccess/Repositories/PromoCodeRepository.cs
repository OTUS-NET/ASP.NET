using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

public class PromoCodeRepository : EfRepository<PromoCode>
{
    public PromoCodeRepository(DataContext dataContext) : base(dataContext) { }

    public override async Task<IEnumerable<PromoCode>> GetAllAsync()
    {
        return await _dbContext.Set<PromoCode>()
           .Include(c => c.PartnerManager)
           .ToListAsync();
    }

    public override async Task<PromoCode> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<PromoCode>()
             .Include(c => c.PartnerManager)
             .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<PromoCode> UpdateAsync(PromoCode promoCode)
    {
        return promoCode;
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        /*var customer = await GetByIdAsync(id);

        if (customer == null)
            return false;

        _dbContext.RemoveRange(customer.PromoCodes);
        _dbContext.Remove(customer);

        await _dbContext.SaveChangesAsync();*/

        return true;
    }
}