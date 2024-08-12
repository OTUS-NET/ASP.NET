using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Service.Employers.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Service.Employers
{
    public interface IEmployeeService
    {
        Task<List<EmployeeShortResponse>> GetEmployeesAsync();

        Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id);

        Task CreateEmployee(EmployeeCreateRequest EmployeeCreateRequest);

        Task DeleteEmployeeAsync(Guid id);

        Task UpdateEmployeeAsync(EmployeeRequest employee);
    }
}
