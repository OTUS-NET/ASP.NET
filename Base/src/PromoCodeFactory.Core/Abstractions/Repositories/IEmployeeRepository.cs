using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.Dtos.Administation;
using System;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee> AddAsync(EmployeeDto dto);

    Task UpdateAsync(Guid id, EmployeeDto dto);
}