using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;
internal class PreferenceEfRepository(PromoCodeFactoryDbContext context) : EfRepository<Preference>(context)
{
    protected override IQueryable<Preference> ApplyIncludes(IQueryable<Preference> query)
    {
        return query.Include(p => p.Customers);
    }
}
