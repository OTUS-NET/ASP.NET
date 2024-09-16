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
        [HttpGet("{id:guid}", Name = "GetEmployeeByIdAsync")]
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
        /// Удалить сотрудника
        /// </summary>
        /// <param name="id">Id сотрудника</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);

            if (existingEmployee == null)
                return NotFound();

            await _employeeRepository.DeleteAsync(id);
            return NoContent();
        }
        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <param name="employeeRequest">Данные нового сотрудника</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] EmployeeRequest employeeRequest)
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = employeeRequest.FirstName,
                LastName = employeeRequest.LastName,
                Email = employeeRequest.Email,
                Roles = employeeRequest.Roles.Select(x => new Role
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                AppliedPromocodesCount = employeeRequest.AppliedPromocodesCount
            };

            await _employeeRepository.CreateAsync(employee);

            var employeeModel = new EmployeeResponse
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return CreatedAtRoute(nameof(GetEmployeeByIdAsync), new { id = employee.Id }, employeeModel);
        }
        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="id">Id сотрудника</param>
        /// <param name="employeeRequest">Данные для обновления сотрудника</param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id, [FromBody] EmployeeRequest employeeRequest)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);

            if (existingEmployee == null)
                return NotFound();
            
            existingEmployee.FirstName = employeeRequest.FirstName;
            existingEmployee.LastName = employeeRequest.LastName;
            existingEmployee.Email = employeeRequest.Email;
            existingEmployee.Roles = employeeRequest.Roles.Select(x => new Role
            {
                Name = x.Name,
                Description = x.Description
            }).ToList();
            existingEmployee.AppliedPromocodesCount = employeeRequest.AppliedPromocodesCount;
            
            await _employeeRepository.UpdateAsync(existingEmployee);

            // Возвращаем обновленные данные сотрудника
            var employeeModel = new EmployeeResponse
            {
                Id = existingEmployee.Id,
                Email = existingEmployee.Email,
                Roles = existingEmployee.Roles.Select(x => new RoleItemResponse
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = existingEmployee.FullName,
                AppliedPromocodesCount = existingEmployee.AppliedPromocodesCount
            };

            return Ok(employeeModel);
        }
    }
}