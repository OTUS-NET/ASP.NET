using System;
using System.Collections.Generic;
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
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync(CreateEmployeeModel model)
        {
            var newEmployee = new Employee()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Roles = new List<Role>(),
                AppliedPromocodesCount = 0
            };

            var createdEmployee = await _employeeRepository.CreateAsync(newEmployee);

            return new EmployeeResponse
            {
                Id = createdEmployee.Id,
                FullName = createdEmployee.FullName,
                Email = createdEmployee.Email,
                Roles = createdEmployee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                AppliedPromocodesCount = createdEmployee.AppliedPromocodesCount
            };
        }

        /// <summary>
        /// Обновить данные о сотруднике
        /// </summary>
        /// <returns></returns>

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id, UpdateEmployeeModel model)
        {
            var newEmployee = new Employee()
            {
                Id = id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Roles = model.Roles
                    .Select(x => FakeDataFactory.Roles.FirstOrDefault(r => r.Id == x.Id))
                    .ToList(),
                AppliedPromocodesCount = model.AppliedPromocodesCount,
            };

            var updatedEmployee = await _employeeRepository.UpdateAsync(id, newEmployee);

            return new EmployeeResponse
            {
                Id = updatedEmployee.Id,
                FullName = updatedEmployee.FullName,
                Email = updatedEmployee.Email,
                Roles = updatedEmployee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                AppliedPromocodesCount = updatedEmployee.AppliedPromocodesCount
            };
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            await _employeeRepository.DeleteAsync(id);
            return Ok();
        }
    }
}