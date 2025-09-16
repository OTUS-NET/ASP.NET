using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Mappers;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController(
        IRepository<Employee> employeeRepository,
        IEmployeeMappers employeeMappers
        )
        : ControllerBase
    {
        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await employeeRepository.GetAllAsync();
            return employees.Select(x => employeeMappers.EmployeeToEmployeeShortResponse.Map(x)).ToList();
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

            var employeeModel = employeeMappers.EmployeeToEmployeeResponse.Map(employee);
            return employeeModel;
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <param name="creationRequest">Данные для создания сотрудника</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployee(EmployeeCreationRequest creationRequest)
        {
            if (creationRequest.Email == null 
                || creationRequest.FirstName == null 
                || creationRequest.LastName == null)
                return BadRequest();
            
            var employee = employeeMappers.EmployeeCreationRequestToEmployee.Map(creationRequest);
            await employeeRepository.AddAsync(employee);
            
            var savedEmployee = await employeeRepository.GetByIdAsync(employee.Id);
            return savedEmployee != null 
                ? Created($"/api/v1//employees/{employee.Id:D}", employeeMappers.EmployeeToEmployeeResponse.Map(employee))
                : StatusCode((int)HttpStatusCode.InternalServerError);
        }
        
        /// <summary>
        /// Удаление сотрудника по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployee(Guid id)
        {
            var employee = await employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound(id);
            
            await employeeRepository.DeleteAsync(employee);
            return Ok(id); 
        }
        
        /// <summary>
        /// Обновление данных сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployee(Guid id, EmployeeUpdateRequest updateRequest)
        {
            var employee = employeeMappers.EmployeeUpdateRequestToEmployee.Map(updateRequest);
            employee.Id = id;
            
            var updatedEmployee = await employeeRepository.UpdateAsync(employee);
            return Ok(employeeMappers.EmployeeToEmployeeResponse.Map(updatedEmployee)); 
        }
    }

    
    
}