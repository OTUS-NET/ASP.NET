using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class CustomerEfRepository(PromoCodeFactoryDbContext context) : EfRepository<Customer>(context)
{
    protected override IQueryable<Customer> ApplyIncludes(IQueryable<Customer> query)
    {
        return query
            .Include(c => c.Preferences)
            .Include(c => c.CustomerPromoCodes);
    }
}
