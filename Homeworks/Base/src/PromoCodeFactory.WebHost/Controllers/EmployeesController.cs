using Mapster;
using Microsoft.AspNetCore.JsonPatch;
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
        private readonly IRepository<Role> _rolesRepository;
        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> rolesRepository)
        {
            _employeeRepository = employeeRepository;
            _rolesRepository = rolesRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x => x.Adapt<EmployeeShortResponse>()).ToList();
            
            return employeesModelList;
        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> AddEmployeeAsync(EmployeeModel emp)
        {
            var roles = (await _rolesRepository.GetAllAsync()).ToDictionary(r=>r.Name, r=>r.Id);

            var employee = emp.Adapt<Employee>();
            foreach (var role in employee.Roles)
            {
                role.Id = roles.GetValueOrDefault(role.Name);
            }

            //TODO: Validate all roles correct
            var newEmployee = await _employeeRepository.AddAsync(employee);

            if (newEmployee == null)
                return NoContent();

            var employeeModel = newEmployee.Adapt<EmployeeResponse>();

            return employeeModel;
        }

        /// <summary>
        /// Получить данные сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = employee.Adapt<EmployeeResponse>();

            return employeeModel;
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(EmployeeModel emp)
        {

            if ((emp.Id == null) || (emp.Id == Guid.Empty))
                return NotFound();

            var roles = (await _rolesRepository.GetAllAsync()).ToDictionary(r => r.Name, r => r.Id);

            var employee = emp.Adapt<Employee>();
            foreach (var role in employee.Roles)
            {
                role.Id = roles.GetValueOrDefault(role.Name);
            }

            var updatedEmployee = await _employeeRepository.UpdateAsync(employee);

            if (updatedEmployee == null)
                return NotFound();

            var employeeModel = updatedEmployee.Adapt<EmployeeResponse>();

            return employeeModel;
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<EmployeeResponse>> PatchEmployeeAsync(Guid id, JsonPatchDocument<Employee> patch)
        {
            var employee = await _employeeRepository.GetAsync(id);

            if (employee == null)
                return NotFound();

            patch.ApplyTo(employee);

            var patchedEmployee = await _employeeRepository.UpdateAsync(employee);

            var employeeModel = patchedEmployee.Adapt<EmployeeResponse>();

            return employeeModel;
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.DeleteAsync(id);

            if (employee == null)
                return NotFound();

            return Ok();
        }
    }
}