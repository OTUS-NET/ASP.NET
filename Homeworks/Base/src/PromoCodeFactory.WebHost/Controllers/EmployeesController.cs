using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
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
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] EmployeeResponse employeeModel)
        {

            string[] name = employeeModel.FullName.Split(' ');
            var employee = new Employee()
            {
                Id = employeeModel.Id,
                Email = employeeModel.Email,
                Roles = employeeModel.Roles.Select(x => new Role()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FirstName = name[0],
                LastName = name[1],
                AppliedPromocodesCount = employeeModel.AppliedPromocodesCount
            };

            await _employeeRepository.AddAsync(employee);
            
            return employeeModel;
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid id, [FromBody] Employee newEmployee)
        {

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            employee.FirstName = newEmployee.FirstName;
            employee.LastName = newEmployee.LastName;
            employee.Email = newEmployee.Email;
            employee.Roles = newEmployee.Roles;
            employee.AppliedPromocodesCount = newEmployee.AppliedPromocodesCount;

            await _employeeRepository.UpdateAsync(newEmployee);
            return Ok();

        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();
            await _employeeRepository.DeleteAsync(id);
            return Ok();

        }
    }
}