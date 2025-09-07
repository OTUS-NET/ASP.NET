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
        /// Добавление нового сотрудника
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> AddEmployeeAsync([FromBody] EmployeeRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName))
                return BadRequest("Поля Email и Name обязательны");

            var existingEmployee = (await _employeeRepository.GetAllAsync())
                .FirstOrDefault(e => e.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

            if (existingEmployee != null)
                return Conflict("Сотрудник с таким email уже существует");

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Roles = request.Roles?.Select(r => new Role { Name = r.Name, Description = r.Description }).ToList()
                        ?? new List<Role>(),
                AppliedPromocodesCount = 0
            };

            var addedEmployee = await _employeeRepository.AddAsync(employee);

            // response
            var response = new EmployeeResponse
            {
                Id = addedEmployee.Id,
                Email = addedEmployee.Email,
                FullName = addedEmployee.FullName,
                Roles = addedEmployee.Roles.Select(r => new RoleItemResponse
                {
                    Name = r.Name,
                    Description = r.Description
                }).ToList(),
                AppliedPromocodesCount = addedEmployee.AppliedPromocodesCount
            };

            return CreatedAtAction("GetEmployeeById", new { id = response.Id }, response);
        }

        /// <summary>
        /// Изменение данных о сотруднике
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id, [FromBody] EmployeeRequest request)
        {
            var employeeDB = await _employeeRepository.GetByIdAsync(id);
            if (employeeDB == null)
                return NotFound();

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName))
                return BadRequest("Поля Email и Name обязательны");

            employeeDB.Email = request.Email;
            employeeDB.FirstName = request.FirstName;
            employeeDB.LastName = request.LastName;

            employeeDB.Roles = request.Roles?.Select(r => new Role { Name = r.Name, Description = r.Description }).ToList()
                                    ?? new List<Role>();
            employeeDB.AppliedPromocodesCount = request.AppliedPromocodesCount;

            var updatedEmployee = await _employeeRepository.UpdateAsync(employeeDB);

            // response
            var response = new EmployeeResponse
            {
                Id = updatedEmployee.Id,
                Email = updatedEmployee.Email,
                FullName = updatedEmployee.FullName,
                Roles = updatedEmployee.Roles.Select(r => new RoleItemResponse
                {
                    Name = r.Name,
                    Description = r.Description
                }).ToList(),
                AppliedPromocodesCount = updatedEmployee.AppliedPromocodesCount
            };

            return Ok(response);
        }
        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var employeeDb = await _employeeRepository.GetByIdAsync(id);
            if (employeeDb == null)
                return NotFound();

            var result = await _employeeRepository.RemoveAsync(id);

            if (!result)
                return StatusCode(500, "Не удалось удалить сотрудника");

            return NoContent();
        }
    }
}