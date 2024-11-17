using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync([Required] Guid id)
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
        /// Добавить нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody][Required] Employee employeeIn)
        {
            try
            {
                if (employeeIn is null)
                    return BadRequest("Переданы неверные параметры для создания нового сотрудника!");

                var employee = await _employeeRepository.CreateAsync(employeeIn);

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

                return StatusCode(StatusCodes.Status201Created, employeeModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }   
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync([FromBody][Required] Employee employeeIn)
        {
            try
            {
                if (employeeIn is null)
                    return BadRequest("Переданы неверные параметры для обновления данных сотрудника!");

                var employee = await _employeeRepository.UpdateAsync(employeeIn);

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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployeeByIdAsync([Required] Guid id)
        {
            try
            {
                bool result = await _employeeRepository.DeleteByIdAsync(id);

                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Сотрудник с таким id не найден!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}