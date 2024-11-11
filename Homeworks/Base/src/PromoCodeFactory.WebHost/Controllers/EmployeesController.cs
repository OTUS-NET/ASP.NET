using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private readonly IRepository<Role> _rolesRepository;

        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> rolesRepository)
        {
            _employeeRepository = employeeRepository;
            _rolesRepository = rolesRepository;
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
        /// Создание нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync(EmployeeCreateRequest request)
        {
            // Проверка корректности email
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(request.Email))
            {
                return BadRequest(new { Error = "Некорректный формат email" });
            }
            
            // Проверка корректности формата Guid для RoleIds
            var validRoleIds = new List<Guid>();
            var invalidRoleIds = new List<string>();

            foreach (var roleId in request.RoleIdList)
            {
                if (Guid.TryParse(roleId, out var validGuid))
                {
                    validRoleIds.Add(validGuid);
                }
                else
                {
                    invalidRoleIds.Add(roleId);
                }
            }

            // Если есть некорректные RoleIds, возвращаем ошибку
            if (invalidRoleIds.Any())
            {
                return BadRequest(new { Error = $"Некорректный формат id ролей" });
            }

            // Получаем роли из базы по валидным Id
            var roles = await _rolesRepository.GetAllAsync();
            var employeeRoles = roles.Where(role => validRoleIds.Contains(role.Id)).ToList();

            // Проверяем, что все роли найдены
            var missingRoleIds = validRoleIds.Except(employeeRoles.Select(r => r.Id)).ToList();
            if (missingRoleIds.Any())
            {
                return BadRequest(new { Error = $"Роли не найдены для Id: {string.Join(", ", missingRoleIds)}" });
            }

            // Создаем нового сотрудника
            var newEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Roles = employeeRoles,
                AppliedPromocodesCount = 0
            };

            await _employeeRepository.AddAsync(newEmployee);
            
            return Ok(new EmployeeResponse
            {
                Id = newEmployee.Id,
                Email = newEmployee.Email,
                FullName = newEmployee.FullName,
                Roles = newEmployee.Roles.Select(role => new RoleItemResponse
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                }).ToList(),
                AppliedPromocodesCount = newEmployee.AppliedPromocodesCount
            });
        }
        
        /// <summary>
        /// Удаление сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(string id)
        {
            // Проверка формата Guid
            if (!Guid.TryParse(id, out var employeeId))
            {
                return BadRequest(new { Error = "Некорректный формат идентификатора." });
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound(new { Error = "Сотрудник не найден." });
            }

            await _employeeRepository.DeleteAsync(employeeId);
            return NoContent();
        }
        
        /// <summary>
        /// Обновление сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid id, EmployeeUpdateRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { Error = "Сотрудник не найден" });
            }

            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(request.Email))
            {
                return BadRequest(new { Error = "Некорректный формат id ролей" });
            }

            var validRoleIds = new List<Guid>();
            var invalidRoleIds = new List<string>();

            foreach (var roleId in request.RoleIdList)
            {
                if (Guid.TryParse(roleId.ToString(), out var validGuid))
                {
                    validRoleIds.Add(validGuid);
                }
                else
                {
                    invalidRoleIds.Add(roleId);
                }
            }

            if (invalidRoleIds.Any())
            {
                return BadRequest(new { Error = $"Некорректный формат id ролей" });
            }

            // Получаем роли из базы по валидным Id
            var roles = await _rolesRepository.GetAllAsync();
            var employeeRoles = roles.Where(role => validRoleIds.Contains(role.Id)).ToList();

            // Проверка существования ролей
            var missingRoleIds = validRoleIds.Except(employeeRoles.Select(r => r.Id)).ToList();
            if (missingRoleIds.Any())
            {
                return BadRequest(new { Error = $"Роли не найдены для id" });
            }

            // Обновляем поля сотрудника
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Roles = employeeRoles;

            await _employeeRepository.UpdateAsync(employee);

            return NoContent();
        }
    }
}