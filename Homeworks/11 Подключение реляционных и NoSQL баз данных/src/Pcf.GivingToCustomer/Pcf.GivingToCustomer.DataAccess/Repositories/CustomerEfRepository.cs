using Microsoft.EntityFrameworkCore;
using Pcf.Common.Ef;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Repositories;

internal class CustomerEfRepository(PromoCodeFactoryDbContext context) : EfRepository<PromoCodeFactoryDbContext, Customer>(context)
{
    protected override IQueryable<Customer> ApplyIncludes(IQueryable<Customer> query)
    {
        return query
            .Include(c => c.Preferences)
            .Include(c => c.CustomerPromoCodes);
    }
}
