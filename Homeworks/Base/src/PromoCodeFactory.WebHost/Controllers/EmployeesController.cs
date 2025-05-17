using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeeShortResponse>))]
        public async Task<IActionResult> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModels = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                });

            return Ok(employeesModels);
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> GetEmployeeByIdAsync(Guid id)
        {
            Employee employee = await _employeeRepository.GetByIdAsync(id);

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

            return Ok(employeeModel);
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EmployeeShortResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var employee = new Employee()
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    AppliedPromocodesCount = 0,
                    Roles = new List<Role>()
                };
            var createdEmployee = await _employeeRepository.AddAsync(employee);
            if (createdEmployee is null)
                return BadRequest("Произошла ошибка при создании сотрудника.");
            var createdEmployeeShortResponse = new EmployeeShortResponse()
            {
                Id = createdEmployee.Id,
                Email = createdEmployee.Email,
                FullName = createdEmployee.FullName
            };
            return Created(string.Empty, createdEmployeeShortResponse);
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> DeleteEmployeeByIdAsync(Guid id)
        {
            Employee employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound("Сотрудник с данным id не найден");
            await _employeeRepository.DeleteAsync(employee);
            return Ok();
        }

        /// <summary>
        /// Редактировать сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request)
        {
            Employee employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound("Сотрудник с данным id не найден");
            
            var employeeUpdated = new Employee()
            {
                Id = id,
                FirstName = string.IsNullOrEmpty(request.FirstName) ? employee.FirstName : request.FirstName,
                LastName = string.IsNullOrEmpty(request.LastName) ? employee.LastName : request.LastName,
                Email = string.IsNullOrEmpty(request.Email) ? employee.Email : request.Email,
                AppliedPromocodesCount = request.AppliedPromocodesCount,
                Roles = employee.Roles
            };
            await _employeeRepository.UpdateAsync(employeeUpdated);
            return Ok();
        }
    }
}