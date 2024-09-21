using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Models.Dto;

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
        /// Создать нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateEmployeeAsync(EmployeeDtoCreate employee)
        {
            var Roles = await _roleRepository.GetAllAsync();
            string Error = employee.Validate(Roles);
            if (!string.IsNullOrEmpty(Error))
            {
                return BadRequest(new ErrorMessage(Error, Request.Path.Value));
            }

            var res = employee.ToEmployee();
            await _employeeRepository.AddAsync(res);

            return res.Id.ToString();
        }

        /// <summary>
        /// Изменить сотрудника 
        /// </summary>
        /// <returns></returns>
        [HttpPut("Change/{id:guid}")]
        public async Task<IActionResult> ChangeEmployeeAsync(Guid id, EmployeeDtoCreate employee)
        {
            var Roles = await _roleRepository.GetAllAsync();
            string Error = employee.Validate(Roles);
            if (!string.IsNullOrEmpty(Error))
            {
                return BadRequest(new ErrorMessage(Error, Request.Path.Value));
            }

            var currEmployee = await _employeeRepository.GetByIdAsync(id);
            if (currEmployee == null)
                return NotFound();
            
            await _employeeRepository.UpdateByIdAsync(id,employee.ToEmployee());

            return Ok(null);
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeByIdAsync(Guid id)
        {
            
            var res=await _employeeRepository.DeleteByIdAsync(id);
            if(!res)
                return NotFound();
            return Ok(null);
        }
    }
}