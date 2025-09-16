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

        private readonly IRepository<Role> _roleRepository;

        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
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
        [HttpGet("{id:guid}", Name = "GetEmployeeById")]
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
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            if (request == null)
                return BadRequest();

            var employee = new Employee
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AppliedPromocodesCount = request.AppliedPromocodesCount,
                Roles = (await _roleRepository.GetAllAsync()).Where(r => request.RolesIds.Contains(r.Id)).ToList()
            };

            await _employeeRepository.AddAsync(employee);

            return CreatedAtRoute("GetEmployeeById", new { id = employee.Id }, null);
        }

        /// <summary>
        /// Обновить данные о сотруднике
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest request)
        {
            if (request == null)
                return BadRequest();

            var employee = await _employeeRepository.GetByIdAsync(request.Id);
            if (employee == null)
                return NotFound();

            employee.Email = request.Email;
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.AppliedPromocodesCount = request.AppliedPromocodesCount;
            employee.Roles = (await _roleRepository.GetAllAsync()).Where(r => request.RolesIds.Contains(r.Id)).ToList();

            await _employeeRepository.UpdateAsync(employee);

            return NoContent();
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployee(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            await _employeeRepository.DeleteAsync(id);

            return NoContent();
        }

    }
}