using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
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
        /// Добавить сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> AddEmployeeAsync(EmployeeModel employee)
        {
            var rols = await _rolesRepository.GetAllAsync();
            var roles = rols.ToList();
            var emp= new Employee()
            {
                FirstName= employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new Role()
                {
                    Id= roles.FirstOrDefault(r => r.Name == x.Name).Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };
            
            //TODO: Validate all roles correct
            var newEmployee = await _employeeRepository.AddAsync(emp);

            if (newEmployee == null)
                return NoContent();

            var employeeModel = new EmployeeResponse()
            {
                Id = newEmployee.Id,
                Email = newEmployee.Email,
                Roles = newEmployee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = newEmployee.FullName,
                AppliedPromocodesCount = newEmployee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Получить данные сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetAsync(id);

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
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(EmployeeModel employee)
        {

            if ((employee.Id == null) || (employee.Id == Guid.Empty))
                return NotFound();

            var rols = await _rolesRepository.GetAllAsync();
            var roles = rols.ToList();

            var emp = new Employee()
            {
                Id= employee.Id.Value,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new Role()
                {
                    Id = roles.FirstOrDefault(r => r.Name == x.Name).Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            var updatedEmployee = await _employeeRepository.UpdateAsync(emp);

            if (updatedEmployee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = updatedEmployee.Id,
                Email = updatedEmployee.Email,
                Roles = updatedEmployee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = updatedEmployee.FullName,
                AppliedPromocodesCount = updatedEmployee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<EmployeeResponse>> PatchEmployeeAsync(Guid id, JsonPatchDocument<Employee> patch)
        {
            var employee = await _employeeRepository.GetAsync(id);

            if (employee == null)
                return NotFound();

            patch.ApplyTo(employee);

            var patchedEmployee = await _employeeRepository.UpdateAsync(employee);

            var employeeModel = new EmployeeResponse()
            {
                Id = patchedEmployee.Id,
                Email = patchedEmployee.Email,
                Roles = patchedEmployee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = patchedEmployee.FullName,
                AppliedPromocodesCount = patchedEmployee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.DeleteAsync(id);

            if (employee == null)
                return NotFound();

            return Ok();
        }
    }
}