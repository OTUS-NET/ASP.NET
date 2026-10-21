using Microsoft.EntityFrameworkCore;
using Pcf.Common.Ef;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Repositories;

internal class PromoCodeEfRepository(PromoCodeFactoryDbContext context) : EfRepository<PromoCodeFactoryDbContext, PromoCode>(context)
{
    protected override IQueryable<PromoCode> ApplyIncludes(IQueryable<PromoCode> query)
    {
        return query.Include(p => p.Preference);
    }
}
