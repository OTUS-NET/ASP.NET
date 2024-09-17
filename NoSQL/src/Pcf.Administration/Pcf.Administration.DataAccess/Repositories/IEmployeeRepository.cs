using Pcf.Administration.Core.Domain.Administration;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee> GetEmployeeByIdAsync(string id);
    Task AddEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(string id, Employee updatedEmployee);
    Task DeleteEmployeeAsync(string id);
}
