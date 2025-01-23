using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Extensions;
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
            var entities = await _employeeRepository.GetAllAsync();
            var response = 
                entities
                .Select(x => x.ToShortResponse())
                .ToList();

            return response;
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
                var entity = await _employeeRepository.GetByIdAsync(id);
                if (entity == default)
                {
                    return NotFound();
                }

                var response = entity.ToResponse();
                return Ok(response);
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
        public async Task<IActionResult> CreateEmployeeIdAsync(CreateEmployeeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var roles = GetRoles(request.RoleIds);
                var entity = request.ToEntity(roles);
                var created = await _employeeRepository.CreateAsync(entity);
                if (created == null)
                {
                    return Problem("Возникла ошибка при добавлении сотрудника.");
                }

                var response = created.ToResponse();
                return Created(nameof(GetEmployeeByIdAsync), response);
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
        public async Task<IActionResult> Edit(EditEmployeeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _employeeRepository.GetByIdAsync(request.Id) == default)
                {
                    return NotFound();
                }

                var roles = GetRoles(request.RoleIds);
                var entity = request.ToEntity(roles);
                var updated = await _employeeRepository.UpdateAsync(entity);
                if (updated == null)
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var entity = await _employeeRepository.GetByIdAsync(id);
            if (entity == default)
            {
                return NotFound();
            }

            if (!await _employeeRepository.DeleteAsync(id))
            {
                return Problem("Возникла ошибка при удалении сотрудника");
            }

            return NoContent();
        }

        private List<Role> GetRoles(IEnumerable<Guid> ids)
        {
            return _roleRepository
                .GetAllAsync()?.Result?
                .Where(x => ids.Contains(x.Id))
                .ToList();
        }
    }
}