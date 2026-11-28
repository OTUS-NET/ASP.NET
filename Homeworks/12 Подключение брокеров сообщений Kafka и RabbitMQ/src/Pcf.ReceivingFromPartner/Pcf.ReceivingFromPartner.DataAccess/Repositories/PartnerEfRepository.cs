using Microsoft.EntityFrameworkCore;
using Pcf.Common.Ef;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.DataAccess.Repositories;

internal class PartnerEfRepository(PromoCodeFactoryDbContext context) : EfRepository<PromoCodeFactoryDbContext, Partner>(context)
{
    protected override IQueryable<Partner> ApplyIncludes(IQueryable<Partner> query)
    {
        return query
            .Include(p => p.PartnerLimits);
    }
}
