using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models.Responses;

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
        /// Get the data of all employees
        /// /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeShortResponse>), 200)]
        public async Task<IEnumerable<EmployeeShortResponse>> GetEmployeesAsync() =>
            (await employeeRepository.GetAllAsync()).Select(mapper.Map<EmployeeShortResponse>);

        /// <summary>
        /// Get employee data by id
        /// Получить данные сотрудника по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmployeeResponse>> Get(Guid id)
        {
            var employee = await employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();
            var employeeModel = mapper.Map<EmployeeResponse>(employee);
            return employeeModel;
        }

        ///<summary>
        /// Добавить нового сотрудника, с указанием роли
        /// Add a new employee, specifying the role
        ///</summary>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] CreateOrEditEmployeeRequest request)
        {
            if (string.IsNullOrEmpty(request.NamesRole)) return BadRequest("Roles is Empty");
            var role = (await roleRepository.GetAllAsync()).FirstOrDefault( r => r.Name == request.NamesRole );
            if (role == null) return NotFound("Role not Found");

            var employee = mapper.Map<Employee>(request);
            employee.Id = Guid.NewGuid();
            employee.Role = role;
            var response = await employeeRepository.CreateAsync(employee);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, mapper.Map<EmployeeResponse>(response));
        }
        ///<summary>
        /// Delete an amployee by id
        /// Удалить работника по id 
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> DeleteEmployeeAsync(Guid id)
        {
            if ((await employeeRepository.GetByIdAsync(id)) == null) return NotFound("Employee id not found");
            await employeeRepository.DeleteAsync(id);
            return Ok(true);
        }
    }
}