using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Models.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.Core.Abstractions.Repositories;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController(IEmployeesRepository employeeRepository, IRepository<Role> roleRepository, IMapper mapper) : ControllerBase
    {
        /// <summary>
        /// Get the data of all employees
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeShortResponse>), 200)]
        public async Task<IEnumerable<EmployeeShortResponse>> GetEmployeesAsync() =>
            (await emploeeRepository.GetAllAsync()).Select(mapper.Map<EmployeeShortResponse>);   
        /// <summary>
        /// Get the employee data by Id
        /// Получить данные сотрудника по id
        /// </summary>
        /// <returns></returns> 

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmployeeResponse>> Get(Guid id)
        {
            var employee = await emploeeRepository.GetByIdAsync(id);

            if (employee == null) 
                return NotFound();
            var employeeModel = mapper.Map<EmployeeResponse>(employee);
            return employeeModel;
        }
        /// <summary>
        /// Add a new employee specifying the role
        /// Добавить нового сотрудника, с указанием роли.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>  
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeResponse), 201)]
        [ProducesResponseType(400)]       
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsynce([FromBody] CreateOrEditEmployeeRequest request)
        {
            if (string.IsNullOrEmpty(request.NamesRole)) return BadRequest("Roles is Empty");
            var role = (await roleRepository.GetAllAsync()).FirstOrDefault(r => r.Name == request.NamesRole);
            if (role == null) return NotFound("Role not Found");

            var employee = mapper.Map<Employee>(request);
            employee.id = Guid.NewGuid();
            employee.Role = role;
            var response = await employeeRepository.CreateAsync(employee);
            return CreatedAtAction(nameof(Get), new { id = response.id }, mapper.Map<EmployeeResponse>(response));
        }

        /// <summary>
        /// Update the employee date by Id 
        /// Обновить данные сотрудника по Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> UpdateEmloyeeAsync(Guid id, [FromBody] CreateOrEditEmployeeRequest request)
        {
            var oldEmployee = await emploeeRepository.GetByIdAsync(id);
            if (oldEmployee == null) return NotFound("Employee id not found");

            var roles = (await roleRepository.GetAllAsync()).Where(r => request.NameRole.Contains(r.Name));
            updateEmployee.Id = id;
            if (!string.IsNullOrEmpty(request.NamesRole)) updateEmployee.Role = oldEmploqyee.Role;
            else
            {
                var roles = (await roleRepository.GetAllAsync()).Where(r => request.NamesRole.Contains(r.Name));
                updateEmployee.Role = roles.First();
            }

            await employeeRepository.UpdateAsync(id, updateEmployee);
            return Ok(true);
        }


    }

}