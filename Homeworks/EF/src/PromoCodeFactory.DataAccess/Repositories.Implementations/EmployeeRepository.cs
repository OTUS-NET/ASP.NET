using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqKit;
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
        IQueryable<Employee> query = GetAll(asNoTracking);

        if (employeeFilterDto?.Names is { Count: > 0 })
        {
            var pb = PredicateBuilder.New<Employee>();
            foreach (var name in employeeFilterDto.Names)
            {
                pb = pb.Or(employee => employee.FirstName + " " + employee.LastName == name);
            }
            query = query.Where(pb);
        }

        return await query.ToListAsync(cancellationToken);
    }
}