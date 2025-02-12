using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Services.Contracts.Employee;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.DataAccess.Repositories.Implementations;

public class EmployeeRepository : EfRepository<Employee, Guid>, IEmployeeRepository
{
    public EmployeeRepository(DatabaseContext databaseContext) : base(databaseContext)
    {
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        EmployeeFilterDto employeeFilterDto = null)
    {
        var query = GetAll(asNoTracking);

        if (employeeFilterDto?.Names is { Count: > 0 })
        {
            query = query.Where(c => employeeFilterDto.Names.Contains(c.FullName));
        }

        return await query.ToListAsync(cancellationToken);
    }
}