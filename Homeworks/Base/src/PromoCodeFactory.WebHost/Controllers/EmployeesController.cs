using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                Roles = employee.Roles?.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList() ?? [],
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        private static EmployeeResponse CreateEmployeeResponse(Employee employee)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles?.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList() ?? [],
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };
        }
        
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployee(EmployeeCreationRequest creationRequest)
        {
            if (creationRequest.Email == null 
                || creationRequest.FirstName == null 
                || creationRequest.LastName == null)
                return BadRequest();
            
            var employee = new Employee()
            {
                Id = Guid.NewGuid(), 
                FirstName = creationRequest.FirstName,
                LastName = creationRequest.LastName,
                Email = creationRequest.Email
            };
            
            await _employeeRepository.AddAsync(employee);
            
            var savedEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
            return savedEmployee != null 
                ? StatusCode((int)HttpStatusCode.Created, CreateEmployeeResponse(employee))
                : StatusCode((int)HttpStatusCode.InternalServerError);
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployee(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound(id);
            
            await _employeeRepository.DeleteAsync(employee);
            return Ok(id); 
        }
        
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployee(Guid id, EmployeeUpdateRequest updateRequest)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound(id);
            
            employee.FirstName = updateRequest.FirstName;
            employee.LastName = updateRequest.LastName;
            employee.Email = updateRequest.Email;
            
            await _employeeRepository.UpdateAsync(employee);
            
            return Ok(CreateEmployeeResponse(employee)); 
        }
    }

    public class EmployeeInformation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
    public class EmployeeCreationRequest : EmployeeInformation { }
    
    public class EmployeeUpdateRequest : EmployeeInformation { }
    
}