using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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

        public EmployeesController(
            IRepository<Employee> employeeRepository,
            IRepository<Role> roleRepository)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            try
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
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                    FullName = employee.FullName,
                    AppliedPromocodesCount = employee.AppliedPromocodesCount
                };

                return Ok(employeeModel);
            }
            catch
            {
                return Problem();
            }
        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEmployeeIdAsync(CreateEmployeeRequest item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var roles = 
                    _roleRepository
                    .GetAllAsync()?.Result?
                    .Where(x => item.RoleIds.Contains(x.Id))
                    .Select(x => new Role()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

                var employee = new Employee()
                {
                    FirstName = item.Firstname,
                    LastName = item.Lastname,
                    Email = item.Email,
                    Roles = roles,
                    AppliedPromocodesCount = item.AppliedPromocodesCount
                };

                var entity = await _employeeRepository.CreateAsync(employee);

                if (entity == null)
                {
                    return Problem("Failed to create employee.");
                }

                var employeeModel = new EmployeeResponse()
                {
                    Id = entity.Id,
                    Email = entity.Email,
                    Roles = entity.Roles?.Select(x => new RoleItemResponse()
                    {
                        Name = x.Name,
                        Description = x.Description
                    }).ToList(),
                    FullName = entity.FullName,
                    AppliedPromocodesCount = entity.AppliedPromocodesCount
                };

                return Created(nameof(GetEmployeeByIdAsync), employeeModel);
            }
            catch (Exception ex)
            {
                return Problem($"Возникла ошибка при добавлении сотрудника. {ex.Message}");
            }
        }

        /// <summary>
        /// Изменить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Edit(EditEmployeeRequest item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _employeeRepository.GetByIdAsync(item.Id) == default)
                {
                    return NotFound();
                }

                var roles =
                    _roleRepository
                    .GetAllAsync()?.Result?
                    .Where(x => item.RoleIds.Contains(x.Id))
                    .Select(x => new Role()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    }).ToList();

                var employee = new Employee()
                {
                    Id = item.Id,
                    FirstName = item.Firstname,
                    LastName = item.Lastname,
                    Email = item.Email,
                    Roles = roles,
                    AppliedPromocodesCount = item.AppliedPromocodesCount
                };

                var entity = await _employeeRepository.UpdateAsync(employee);
                
                if (entity == null)
                {
                    return Problem("Возникла ошибка при изменении данных сотрудника.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem($"Возникла ошибка при изменении данных сотрудника. {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            if (!await _employeeRepository.DeleteAsync(id))
            {
                return Problem("An error occurred while creating the employee.");
            }

            return Ok();
        }
    }
}