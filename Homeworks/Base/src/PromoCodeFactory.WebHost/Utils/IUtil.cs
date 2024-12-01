using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Utils
{
    public interface IUtil
    {
        Task<EmployeeResponse?> UpdateEmplAsync(Guid id, EmployeeCreateDto employeeData, IRepository<Role> roleRepository, IRepository<Employee> employeeRepository, CancellationToken cancellationToken = default);
        Task<List<EmployeeShortResponse>> CreateEmplAsync(EmployeeCreateDto employeeData, IRepository<Role> roleRepository, IRepository<Employee> employeeRepository, CancellationToken cancellationToken = default);
        EmployeeResponse MapEmployeeToEmployeeResponse(Employee employee);
    }
}
