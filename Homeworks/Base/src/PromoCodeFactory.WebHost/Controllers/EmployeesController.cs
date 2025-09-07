using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
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

            return employees.Select(EmployeeMapper.ToShortResponse).ToList();
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

            return EmployeeMapper.ToResponse(employee);
        }
        
        /// <summary>
        /// Создать сотрудника
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] EmployeeAddRequest request)
        {
            var employee = EmployeeMapper.ToEntity(request);
            
            employee.Id = Guid.NewGuid();
            
            await _employeeRepository.AddAsync(employee);
            
            return EmployeeMapper.ToResponse(employee);
        }
        
        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id,
            [FromBody] EmployeeUpdRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            if (request.FirstName != null)
            {
                employee.FirstName = request.FirstName;
            }

            if (request.LastName != null)
            {
                employee.LastName = request.LastName;
            }

            if (request.Email != null)
            {
                employee.Email = request.Email;
            }
            
            await _employeeRepository.UpdateAsync(id, employee);
            return EmployeeMapper.ToResponse(employee);
        }
        
        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> DeleteEmployeeAsync(Guid id)
        {
            var isDeleted = await _employeeRepository.DeleteAsync(id);
            return isDeleted ? Ok(true) : NotFound();
        }
    }
}