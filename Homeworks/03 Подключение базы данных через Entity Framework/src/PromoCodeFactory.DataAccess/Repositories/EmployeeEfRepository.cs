using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EmployeeEfRepository(PromoCodeFactoryDbContext context) : EfRepository<Employee>(context)
{
    protected override IQueryable<Employee> ApplyIncludes(IQueryable<Employee> query)
    {
        return query
            .Include(p => p.Role);
    }
}
