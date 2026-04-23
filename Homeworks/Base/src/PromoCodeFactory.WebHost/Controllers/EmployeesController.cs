using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<List<EmployeeShortResponse>>> GetEmployeesAsync()
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync();

                var employeesModelList = employees.Select(x =>
                    new EmployeeShortResponse()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        FullName = x.FullName,
                    }).ToList();

                return Ok(employeesModelList);
            }
            catch(Exception)
            {
                return Problem(
                    title: "Ошибка при получении списка сотрудников",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            try
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

                return Ok(employeeModel);
            }
            catch (Exception)
            {
                return Problem(
                    title: "Ошибка при получении данных сотрудника", 
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Создать сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] Employee employee)
        {
            try
            {
                var createdEmployee = await _employeeRepository.CreateAsync(employee);

                var employeeModel = new EmployeeResponse()
                {
                    Id = createdEmployee.Id,
                    FullName = createdEmployee.FullName,
                    Email = createdEmployee.Email,
                    Roles = createdEmployee.Roles.Select(x => new RoleItemResponse()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                    AppliedPromocodesCount = createdEmployee.AppliedPromocodesCount
                };

                return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { id = createdEmployee.Id }, employeeModel);
            }
            catch(Exception) 
            {
                return Problem(
                    title: "Ошибка при создании сотрудника",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateEmployeeAsync(Guid id, [FromBody] Employee employee)
        {
            try
            {
                employee.Id = id;

                var updated = await _employeeRepository.UpdateAsync(employee);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch(Exception)
            {
                return Problem(
                    title: "Ошибка при обновлении сотрудника",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            try
            {
                var deleted = await _employeeRepository.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception)
            {
                return Problem(
                    title: "Ошибка при удалении сотрудника",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }


    }
}