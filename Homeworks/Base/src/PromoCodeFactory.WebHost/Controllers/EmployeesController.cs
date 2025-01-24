using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeesService _employeesService;

        public EmployeesController(IEmployeesService employeesService)
        {
            _employeesService = employeesService;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeesService.GetAllAsync();

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeesService.GetByIdAsync(id);

            if (employee == null)
                return NotFound($"No Employee with Id {id} found");

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Id = x.Id,
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
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeResponse>> CreateAsync([FromBody] CreateOrUpdateEmployeeRequest request)
        {
            var newEmployee = await _employeesService.CreateAsync(request);
            
            if (newEmployee == null)
                return BadRequest("Unable to create Employee");
            
            var employeeModel = new EmployeeResponse()
            {
                Id = newEmployee.Id,
                Email = newEmployee.Email,
                Roles = newEmployee.Roles.Select(x => new RoleItemResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = newEmployee.FullName,
                AppliedPromocodesCount = newEmployee.AppliedPromocodesCount
            };
            
            return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { id = newEmployee.Id }, employeeModel);
        }
        
        /// <summary>
        /// Изменить существующего сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateOrUpdateEmployeeRequest request)
        {
            bool isUpdated = await _employeesService.UpdateAsync(id, request);

            if (!isUpdated)
                return BadRequest($"Error updating Employee with Id {id}");

            return NoContent();
        }
        
        /// <summary>
        /// Удалить существующего сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            bool isDeleted = await _employeesService.DeleteAsync(id);

            if (!isDeleted)
                return BadRequest($"Error deleting Employee with Id {id}");

            return NoContent();
        }
    }
}