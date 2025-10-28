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
        /// Создать нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateEmployeeAsync([FromBody] EmployeeAddRequest request)
        {
            var newEmployee = new Employee
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Roles = new()
            };

            var guid = await _employeeRepository.AddAsync(newEmployee);

            return Ok(guid);
        }

        /// <summary>
        /// Обновить данные переданного сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync([FromBody] EmployeeUpdateRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.Id);

            if (employee == null)
                return NotFound();

            if (request.Email != null)
                employee.Email = request.Email;

            if (request.FirstName != null)
                employee.FirstName = request.FirstName;

            if (request.LastName != null)
                employee.LastName = request.LastName;

            await _employeeRepository.UpdateAsync(employee);
            return await GetEmployeeByIdAsync(employee.Id);
        }

        /// <summary>
        /// Удалить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> DeleteEmployeeAsync(Guid id)
        {
            return Ok(await _employeeRepository.RemoveAsync(id));
        }
    }
}