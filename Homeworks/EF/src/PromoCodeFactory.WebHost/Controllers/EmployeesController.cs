using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class EmployeesController(ILogger<EmployeesController> logger, IRepository<Employee> employeeRepository)
        : ControllerBase
    {
        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<EmployeeShortResponse>>> GetEmployeesAsync()
        {
            try
            {
                var employees = await employeeRepository.GetAllAsync();

                var employeesModelList = employees.Select(x =>
                    new EmployeeShortResponse()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        FullName = x.FullName,
                    }).ToList();

                return Ok(employeesModelList);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error get employees list: [{msg}]", e.Message);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Получить данные сотрудника по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            try
            {
                var employee = await employeeRepository.GetByIdAsync(id);

                if (employee == null)
                    return NotFound();

                var employeeModel = new EmployeeResponse()
                {
                    Id = employee.Id,
                    Email = employee.Email,
                    Role = new RoleItemResponse()
                    {
                        Name = employee.Role.Name,
                        Description = employee.Role.Description
                    },
                    FullName = employee.FullName,
                    AppliedPromocodesCount = employee.AppliedPromocodesCount
                };

                return employeeModel;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error get employee with id [{id:D}] list: [{msg}]", id, e.Message);
                return StatusCode(500, e.Message);
            }
        }
    }
}