using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Models.Employees;
using PromoCodeFactory.WebHost.Models.Roles;

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
        private readonly IRepository<Role> _roleRepository;

        public EmployeesController(
            IRepository<Employee> employeeRepository,
            IRepository<Role> roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeShortResponse>>> GetEmployeesAsync()
        {
            //TODO: Возвращать код ошибки, если поймали исключение
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
        [HttpGet("{id:guid}")]
        [ProducesResponseType<EmployeeResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            
            if (employee == null)
                return NotFound();

            var employeeModel = await employee.ToDtoAsync(_roleRepository);

            return Ok(employeeModel);
        }

        /// <summary>
        /// Создать сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType<EmployeeResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEmployeeAsync([FromBody] EmployeeCreateRequest employee)
        {
            var employeeResult = await _employeeRepository.CreateAsync(employee.ToEntity());

            if (employeeResult == null)
                return BadRequest();

            var employeeModel = await employeeResult.ToDtoAsync(_roleRepository);

            //TODO: correct URI
            return Created("api/v1/Employees", employeeModel);
        }

        /// <summary>
        /// Изменить данные сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType<EmployeeResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEmployeeAsync([FromBody] EmployeeRequest employee)
        {
            var employeeResult = await _employeeRepository.UpdateAsync(employee.ToEntity());

            if (employeeResult == null)
                return BadRequest();

            var employeeModel = await employeeResult.ToDtoAsync(_roleRepository);

            return Ok(employeeModel);
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEmployeeAsync([FromBody] Guid employeeId)
        {
            var employeeResult = await _employeeRepository.DeleteAsync(employeeId);

            if (employeeResult == Guid.Empty)
                return BadRequest();

            return NoContent();
        }
    }
}