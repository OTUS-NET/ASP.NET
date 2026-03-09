using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class PartnerEfRepository(PromoCodeFactoryDbContext context) : EfRepository<Partner>(context)
{
    protected override IQueryable<Partner> ApplyIncludes(IQueryable<Partner> query)
    {
        return query
            .Include(p => p.Manager)
            .Include(p => p.PartnerLimits);
    }
}
