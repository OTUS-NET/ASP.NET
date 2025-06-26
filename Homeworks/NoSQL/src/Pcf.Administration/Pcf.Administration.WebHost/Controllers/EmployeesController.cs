using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Pcf.Administration.WebHost.Models;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController
        : ControllerBase
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
        /// Получить данные сотрудника по id
        /// </summary>
        /// <param name="id">Id сотрудника, например <example>451533d5-d8d5-4a11-9c7b-eb9f14e1a32f</example></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(string id)
        {
            var employee = await _employeeRepository.GetByIdAsync(new ObjectId(id));

            if (employee == null)
                return NotFound();

            var role = await _roleRepository.GetByIdAsync(employee.RoleId);

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Role = new RoleItemResponse()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                },
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }
        
        /// <summary>
        /// Обновить количество выданных промокодов
        /// </summary>
        /// <param name="id">Id сотрудника, например <example>451533d5-d8d5-4a11-9c7b-eb9f14e1a32f</example></param>
        /// <returns></returns>
        [HttpPost("{id}/appliedPromocodes")]
        
        public async Task<IActionResult> UpdateAppliedPromocodesAsync(string id)
        {
            var employee = await _employeeRepository.GetByIdAsync(new ObjectId(id));

            if (employee == null)
                return NotFound();

            employee.AppliedPromocodesCount++;

            await _employeeRepository.UpdateAsync(employee);

            return Ok();
        }
    }
}