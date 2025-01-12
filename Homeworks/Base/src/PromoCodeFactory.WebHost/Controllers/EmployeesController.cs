using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;
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
        /// Добавить нового сотрудника
        /// </summary>
        /// <param name="employeeRequest">Данные для нового сотрудника</param>
        /// <returns>Данные добавленного сотрудника</returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> AddEmployeeAsync([FromBody] EmployeeResponse employeeRequest)
        {
            // Получаем список ролей на основе переданных данных
            var roles = employeeRequest.Roles.Select(roleRequest =>
            {
                var role = FakeDataFactory.Roles.FirstOrDefault(r => r.Id == roleRequest.Id);
                return role;
            }).Where(r => r != null).ToList();

            if (!roles.Any())
            {
                return BadRequest("Одна или несколько указанных ролей не найдены.");
            }

            var newEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = employeeRequest.FullName.Split(' ')[0],
                LastName = employeeRequest.FullName.Split(' ')[1],
                Email = employeeRequest.Email,
                Roles = roles,
                AppliedPromocodesCount = employeeRequest.AppliedPromocodesCount
            };

            await _employeeRepository.AddAsync(newEmployee);

            return Ok(new EmployeeResponse
            {
                Id = newEmployee.Id,
                FullName = newEmployee.FullName,
                Email = newEmployee.Email,
                Roles = newEmployee.Roles.Select(r => new RoleItemResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToList(),
                AppliedPromocodesCount = newEmployee.AppliedPromocodesCount
            });
        }

        /// <summary>
        /// Изменение данных о сотруднике
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="employee">Сотрудник</param>
        /// <returns>Мрдель сотрудника</returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id, Employee employee)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Roles = employee.Roles;
            existingEmployee.AppliedPromocodesCount = employee.AppliedPromocodesCount;

            await _employeeRepository.UpdateAsync(existingEmployee);

            var employeeModel = new EmployeeResponse
            {
                Id = existingEmployee.Id,
                FullName = existingEmployee.FullName,
                Email = existingEmployee.Email,
                Roles = existingEmployee.Roles.Select(r => new RoleItemResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToList(),
                AppliedPromocodesCount = existingEmployee.AppliedPromocodesCount
            };

            return Ok(employeeModel);
        }

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await _employeeRepository.DeleteAsync(id);
            return Ok();
        }
    }
}