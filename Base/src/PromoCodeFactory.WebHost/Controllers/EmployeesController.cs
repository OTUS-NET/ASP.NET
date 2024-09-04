using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.Models.Dto;
using PromoCodeFactory.Core.Utils;
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
        private readonly ILogger<Employee> _logger;

        public EmployeesController(IRepository<Employee> employeeRepository, ILogger<Employee> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        public async Task<ActionResult<List<EmployeeShortResponse>>> GetEmployeesAsync()
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

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(CreateResponse(employee));
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync(EmployeeDto dto) // CreateEmployeeRequest?
        {
            if (!ModelState.IsValid) return BadRequest("Incorrect data.");
            var employee = dto.ToEmployee();
            await _employeeRepository.AddAsync(employee);

            return Ok(CreateResponse(employee));
            //return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { Id = employee.Id });
        }

        /// <summary>
        /// Обновить информацию о сотруднике
        /// </summary>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync( // UpdateEmployeeRequest?
            [FromQuery] Guid id,
            [FromBody] EmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest("Incorrect data.");
            var employee = dto.ToEmployee(id);
            await _employeeRepository.UpdateByIdAsync(id, employee);

            return Ok(CreateResponse(employee));
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("delete/{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployeeByIdAsync(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest("Incorrect data.");

            await _employeeRepository.DeleteByIdAsync(id);

            return Ok(null);
        }

        private static EmployeeResponse CreateResponse(Employee employee) => 
            new EmployeeResponse()
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
    }
}