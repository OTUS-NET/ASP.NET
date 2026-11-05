using Microsoft.EntityFrameworkCore;
using Pcf.Common.Ef;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.DataAccess.Repositories;

internal class PromoCodeEfRepository(PromoCodeFactoryDbContext context) : EfRepository<PromoCodeFactoryDbContext, PromoCode>(context)
{
    protected override IQueryable<PromoCode> ApplyIncludes(IQueryable<PromoCode> query)
    {
        return query
            .Include(p => p.Partner)
            .Include(p => p.Preference);
    }
}
