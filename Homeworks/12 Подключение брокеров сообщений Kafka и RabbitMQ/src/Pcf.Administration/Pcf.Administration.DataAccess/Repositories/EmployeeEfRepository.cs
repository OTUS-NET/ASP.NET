using Microsoft.EntityFrameworkCore;
using Pcf.Administration.Core.Domain;
using Pcf.Common.Ef;

namespace Pcf.Administration.DataAccess.Repositories;

internal class EmployeeEfRepository(PromoCodeFactoryDbContext context) : EfRepository<PromoCodeFactoryDbContext, Employee>(context)
{
    protected override IQueryable<Employee> ApplyIncludes(IQueryable<Employee> query)
    {
        return query
            .Include(p => p.Role);
    }
}
