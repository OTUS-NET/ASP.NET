using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Data;
using System.Linq;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

using PromoCodeFactory.WebHost.Utils;

namespace PromoCodeFactory.WebHost.Utils
{
    public class MyUtils : IUtil
    {
        //public async Task<ActionResult<EmployeeResponse>> UpdateEmplAsync(Guid id, EmployeeCreateDto employeeData, IRepository<Role> roleRepository, IRepository<Employee> employeeRepository, CancellationToken cancellationToken = default)
        public async Task<EmployeeResponse?> UpdateEmplAsync(Guid id, EmployeeCreateDto employeeData, IRepository<Role> roleRepository, IRepository<Employee> employeeRepository, CancellationToken cancellationToken = default)
        {
            var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);

            if (employee == null)
                return null;
            else
            {

                var role = await roleRepository.GetByIdAsync(employeeData.RoleId, cancellationToken);

                if (role == null)
                    return null;
                else
                {

                    employee.FirstName = employeeData.FirstName;
                    employee.LastName = employeeData.LastName;
                    employee.Email = employeeData.Email;
                    employee.Roles = new List<Role>()
                       {role};

                    var newEmployee = await employeeRepository.ReplaceAsync(new List<Employee>() { employee }, employee.Id, cancellationToken);

                    if (newEmployee != null)
                    {
                        var employeeModel = new EmployeeResponse()
                        {
                            Id = newEmployee.Id,
                            Email = newEmployee.Email,
                            Roles = newEmployee.Roles.Select(x => new RoleItemResponse()
                            {
                                Name = x.Name,
                                Description = x.Description,
                                Id = x.Id
                            }).ToList(),
                            FullName = newEmployee.FullName,
                            AppliedPromocodesCount = newEmployee.AppliedPromocodesCount
                        };

                        return employeeModel;
                    }
                    else
                        return null;

                }
            }
        }

        public async Task<List<EmployeeShortResponse>> CreateEmplAsync(EmployeeCreateDto employeeData, IRepository<Role> roleRepository, IRepository<Employee> employeeRepository, CancellationToken cancellationToken = default)
        {
            var role = await roleRepository.GetByIdAsync(employeeData.RoleId, cancellationToken);

            var empl = new Employee()
            {
                Id = Guid.NewGuid(),
                Email = employeeData.Email,
                FirstName = employeeData.FirstName,
                LastName = employeeData.LastName,
                Roles = new List<Role>()
                       {
                        role
                        },
                AppliedPromocodesCount = 0
            };


            var emplList = new List<Employee>() { empl };
            var employees = await employeeRepository.CreateAsync(emplList, cancellationToken);


            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;

        }

        public EmployeeResponse MapEmployeeToEmployeeResponse(Employee employee)
        {
            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description,
                    Id = x.Id
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

    }
}
