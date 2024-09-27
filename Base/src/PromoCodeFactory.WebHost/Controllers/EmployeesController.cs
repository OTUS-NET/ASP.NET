using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models.Dto;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Utils;
using System.Threading;
using PromoCodeFactory.Core.Abstractions.Services;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IService<Employee> _employeeService;

        public EmployeesController(IService<Employee> employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        public async Task<ActionResult<List<EmployeeShortResponse>>> GetEmployeesAsync(CancellationToken cts)
        {
            var employees = await _employeeService.GetAllAsync(cts);
            var employeesModelList = employees.ToEmployeeShortResponse();

            return Ok(employeesModelList);
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id, CancellationToken cts)
        {
            var employee = await _employeeService.GetByIdAsync(id, cts);
            return Ok(employee.ToEmployeeResponse());
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync(EmployeeDto dto, CancellationToken cts) // CreateEmployeeRequest?
        {
            var employee = dto.ToEmployee();
            await _employeeService.AddAsync(employee, cts);

            return Ok(employee.ToEmployeeResponse());
            //return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { Id = employee.Id }, employee);
        }

        /// <summary>
        /// Обновить информацию о сотруднике
        /// </summary>
        /// <returns></returns>
        [HttpPost("update/{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync( // UpdateEmployeeRequest?
            Guid id,
            EmployeeDto dto,
            CancellationToken cts)
        {
            var employee = dto.ToEmployee(id);
            await _employeeService.UpdateByIdAsync(id, employee, cts);

            return Ok(employee.ToEmployeeResponse());
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("delete/{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployeeByIdAsync(Guid id, CancellationToken cts)
        {
            await _employeeService.DeleteByIdAsync(id, cts);

            return Ok(null);
        }
    }
}