using PromoCodeFactory.Core.Abstractions.Models.Employees;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Abstractions.Repositories.Interfaces.Employees
{
    public interface IEmployeeRepository : ICRUDRepository<Employee, EmployeeCreateDto, EmployeeUpdateDto>
    {
    }
}
