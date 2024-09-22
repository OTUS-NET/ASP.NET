using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class EmployeesController(IRepository<Employee, Guid> employeeRepository, IRepository<Role, Guid> roleRepository, IMapper mapper) : ControllerBase
    {
        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await employeeRepository.GetAllAsync();
            var employeesModelList = employees.Select(mapper.Map<EmployeeShortResponse>);
            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();
            var employeeModel = mapper.Map<EmployeeResponse>(employee);
            return employeeModel;
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmployeeResponse), 201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] CreateEmployeeRequest request)
        {
            if (request.NamesRoles.Count() == 0) return BadRequest("Roles is Empty");
            var roles = (await roleRepository.GetAllAsync()).Where(r => request.NamesRoles.Contains(r.Name));
            if (roles.Count() == 0) return NotFound("Role not Found");

            var employee = mapper.Map<Employee>(request);
            employee.Id = Guid.NewGuid();
            employee.Role = roles.First();
            var response = await employeeRepository.CreateAsync(employee);
            return mapper.Map<EmployeeResponse>(response);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            var oldEmploqyee = await employeeRepository.GetByIdAsync(id);
            if (oldEmploqyee == null) return NotFound("Employee id not found");
            
            var updateEmployee = mapper.Map<Employee>(request);
            updateEmployee.Id = id;
            if (!string.IsNullOrEmpty(request.NamesRole) ) updateEmployee.Role = oldEmploqyee.Role;
            else
            {
                var roles = (await roleRepository.GetAllAsync()).Where(r => request.NamesRole.Contains(r.Name));
                updateEmployee.Role = roles.First();
            }           

            await employeeRepository.UpdateAsync(id, updateEmployee);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            if ((await employeeRepository.GetByIdAsync(id)) == null) return NotFound("Employee id not found");
            await employeeRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}