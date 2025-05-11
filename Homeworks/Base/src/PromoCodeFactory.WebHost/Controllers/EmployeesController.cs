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
        /// Создать сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateAsync([FromBody] EmployeeRequest request)
        {
            var newEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Roles = request.Roles,
                AppliedPromocodesCount = request.AppliedPromocodesCount
            };

            var employee = await _employeeRepository.CreateAsync(newEmployee);
            
            return Ok(new EmployeeResponse
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
            });
        }

        /// <summary>
        /// Обновить данные сотрудника по id
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateAsync(Guid id, [FromBody] EmployeeRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound("Сотрудник не найден");
            
            var updatedEmployee = new Employee
            {
                Id = id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Roles = request.Roles,
                AppliedPromocodesCount = request.AppliedPromocodesCount
            };
            
            var result = await _employeeRepository.UpdateAsync(updatedEmployee);
            
            if (result == null)
                return StatusCode(500, "Не удалось обновить данные сотрудника");

            return Ok(new EmployeeResponse
            {
                Id = result.Id,
                Email = result.Email,
                Roles = result.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = result.FullName,
                AppliedPromocodesCount = result.AppliedPromocodesCount
            });
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound("Сотрудник не найден");   
            }

            await _employeeRepository.DeleteAsync(id);
            
            return NoContent();
        }
    }
}