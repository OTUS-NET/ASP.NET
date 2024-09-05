using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Dtos.Administation;
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
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
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
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var response = new EmployeeResponse(employee);

            return Ok(response);
        }

        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync(EmployeeDto dto)
        {
            var employee = await _employeeRepository.AddAsync(dto);
            var response = new EmployeeResponse(employee);

            return Ok(response);
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateEmployeeAsync(Guid id, EmployeeDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            await _employeeRepository.UpdateAsync(id, dto);

            return Ok();
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            await _employeeRepository.DeleteAsync(id);

            return Ok();
        }
    }
}