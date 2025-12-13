using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Models.Employees;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Repositories.Interfaces.Employees;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [HttpPost("Create")]
        public async Task<Employee> CreateAsync([FromBody] EmployeeCreateDto employeeCreate, CancellationToken cancellationToken) =>
            await this._employeeRepository.AddAsync(employeeCreate, cancellationToken);

        [HttpPatch("Update/{id:guid}")]
        public async Task<Employee> UpdateAsync(Guid id, [FromBody] EmployeeUpdateDto employeeUpdate, CancellationToken cancellationToken) =>
            await this._employeeRepository.UpdateAsync(id, employeeUpdate, cancellationToken);

        [HttpDelete("Delete")]
        public async Task<ActionResult<Guid>> DeleteAsync(Guid id)
        {
            try
            {
                return await this._employeeRepository.DeleteAsync(id);
            }
            catch (NotFoundEntityException ex)
            {
                return NotFound();
            }
        }
    }
}