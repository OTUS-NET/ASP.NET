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
        [HttpPost("create")]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDTO)
        {
            var newEntity = new Employee()
            {
                FirstName = employeeCreateDTO.FirstName,
                LastName = employeeCreateDTO.LastName,
                Email = employeeCreateDTO.Email,
                AppliedPromocodesCount = employeeCreateDTO.AppliedPromocodesCount,
                Roles = new List<Role>(),
            };

            var result = await _employeeRepository.CreateAsync(newEntity);

            return await GetEmployeeByIdAsync(result.Id);
        }


        /// <summary>
        /// Обновить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(EmployeeUpdateDTO employeeUpdateDTO)
        {
            var entity = await _employeeRepository.GetByIdAsync(employeeUpdateDTO.Id);

            if (entity == null)
                return BadRequest();

            entity.Email = employeeUpdateDTO.Email;
            entity.FirstName = employeeUpdateDTO.FirstName;
            entity.LastName = employeeUpdateDTO.LastName;
            entity.AppliedPromocodesCount = employeeUpdateDTO.AppliedPromocodesCount;

            var result = await _employeeRepository.UpdateAsync(entity);

            return await GetEmployeeByIdAsync(result.Id);
        }


        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpGet("delete/{id}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            var entity = await _employeeRepository.GetByIdAsync(id);

            if (entity == null)
                return BadRequest();
            
            var deleted = await _employeeRepository.DeleteAsync(id);
            if (deleted)
                return Ok("Сотрудник удален");

            return BadRequest();
        }

    }
}