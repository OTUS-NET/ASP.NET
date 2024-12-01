using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Utils;



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
        private readonly IUtil _util;

        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> roleRepository, IUtil util)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
            _util = util; 

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
            var employee = await _employeeRepository.GetByIdAsync(id, HttpContext.RequestAborted);

            if (employee == null)
                return NotFound();

            return _util.MapEmployeeToEmployeeResponse(employee);
        }
        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var result = await _employeeRepository.DeleteByIdAsync(id, HttpContext.RequestAborted);

            return Ok(result);

        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<EmployeeShortResponse>> CreateEmployeeAsync(EmployeeCreateDto employeeData)
        {
            //return await CreateEmplAsync(employeeData, _roleRepository, _employeeRepository, HttpContext.RequestAborted);
            return await _util.CreateEmplAsync(employeeData, _roleRepository, _employeeRepository, HttpContext.RequestAborted);

        }


        /// <summary>
        /// Изменить данные сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")] 
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id, [FromBody] EmployeeCreateDto employeeData)
        {
            var result = await _util.UpdateEmplAsync(id, employeeData, _roleRepository, _employeeRepository, HttpContext.RequestAborted);

            if (result == null)
                return NotFound();
            else
                return result;
        }



    }
}