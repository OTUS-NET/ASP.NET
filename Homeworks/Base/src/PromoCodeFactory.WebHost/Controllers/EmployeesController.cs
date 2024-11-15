using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Role> _roleRepository;

        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;

            //

        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

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
        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<List<EmployeeShortResponse>> DeleteEmployeeAsync(Guid id)
        {
            var employees = await _employeeRepository.DeleteByIdAsync(id);

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;

        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<EmployeeShortResponse>> CreateEmployeeAsync([FromBody] EmployeeCreateDto employeeData)
        {

            var role = await _roleRepository.GetByIdAsync(employeeData.RoleId);

            var empl = new Employee()
            {
                //Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a320"),
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
            var employees = await _employeeRepository.CreateAsync(emplList);


            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;

        }

        /// <summary>
        /// Изменить данные сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")] 
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id, [FromBody] EmployeeCreateDto employeeData)
        {
            
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();
            else
            {

                var role = await _roleRepository.GetByIdAsync(employeeData.RoleId);

                if (role == null)
                    return NotFound();
                else
                {

                    employee.FirstName = employeeData.FirstName;
                    employee.LastName = employeeData.LastName;
                    employee.Email = employeeData.Email;
                    employee.Roles = new List<Role>()
                       {role};

                    var newEmployee = await _employeeRepository.ReplaceAsync(new List<Employee>() { employee }, employee.Id);

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
                        return NotFound();

                }
            }

        }



    }
}