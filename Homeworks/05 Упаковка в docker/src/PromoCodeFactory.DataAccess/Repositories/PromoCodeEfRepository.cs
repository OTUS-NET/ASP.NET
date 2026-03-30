using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class PromoCodeEfRepository(PromoCodeFactoryDbContext context) : EfRepository<PromoCode>(context)
{
    protected override IQueryable<PromoCode> ApplyIncludes(IQueryable<PromoCode> query)
    {
        return query
            .Include(p => p.Partner)
                .ThenInclude(partner => partner.Manager)
            .Include(p => p.Preference);
    }
}
