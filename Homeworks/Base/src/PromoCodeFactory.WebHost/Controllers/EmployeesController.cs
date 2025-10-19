using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Models.Employee;

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
        /// Добавить сотрудника
        /// </summary>
        /// <param name="employeeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync(CreateEmployeeRequest employeeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ValidationState);
            }
            var role = _roleRepository.GetByIdAsync(employeeRequest.RoleId);
            if (role == null)
            {
                return BadRequest("Role not found");
            }
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Email = employeeRequest.Email,
                FirstName = employeeRequest.FirstName,
                LastName = employeeRequest.LastName,
                AppliedPromocodesCount = employeeRequest.AppliedPromocodesCount,
                Roles = new List<Role>()
                { 
                    role.Result
                }
            };

            var createdEmployee = await _employeeRepository.AddAsync(employee);
            if (createdEmployee == null)
            {
                return BadRequest();
            }

            return Created("", employee);
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var employee = _employeeRepository.GetByIdAsync(id);
            if (employee.Result == null)
            {
                return BadRequest("Employee not found");
            }

            await _employeeRepository.DeleteAsync(employee.Result);

            return Ok();
        }

        /// <summary>
        /// Обновить данные по сотруднику
        /// </summary>
        /// <param name="id"></param>
        /// <param name="employeeRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync(Guid id, UpdateEmployeeRequest employeeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ValidationState);
            }

            var employee = _employeeRepository.GetByIdAsync(id);
            if (employee.Result == null)
            {
                return BadRequest("Employee not found");
            }

            employee.Result.Email = employeeRequest.Email;
            employee.Result.FirstName = employeeRequest.FirstName;
            employee.Result.LastName = employeeRequest.LastName;

            await _employeeRepository.UpdateAsync(employee.Result);

            return Ok();
        }
    }
}