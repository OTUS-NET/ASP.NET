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
            Employee employee = await _employeeRepository.GetByIdAsync(id);

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
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Добавление нового работника
        /// </summary>
        /// <returns>Id нового работника</returns>
        // [ArrayInput("nn")]   
        [HttpGet("Create/fName:{fName}&lName{lName}&email:{email}&idsRoles")]
        public async Task<ActionResult<Guid>> Create(string fName, string lName, string email, [FromQuery]Guid[]? idsRoles)
        {
            Employee employee = new Employee();
            employee.Id = Guid.NewGuid();
            employee.FirstName = fName;
            employee.LastName = lName;
            employee.Email = email;

            IEnumerable<Role> roles = await _rolesRepository.GetAllAsync();
            employee.Roles = roles.Where(r => idsRoles.Contains(r.Id)).ToList();

            await _employeeRepository.AddAsync(employee);
            return employee.Id;
        }

        /// <summary>
        /// Удаление работника
        /// </summary>
        [HttpGet("Delete/id:{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            bool result = await _employeeRepository.RemoveByIdAsync(id);
            if(!result)
                return NotFound();
            return true;
        }

        /// <summary>
        /// Обновление данных работника
        /// </summary>
        [HttpGet("Update/id:{id}&fName:{fName}&lName{lName}&email:{email}&idsRoles")]
        public async Task<ActionResult<bool>> Update(Guid id, string fName, string lName, string email, [FromQuery]Guid[]? idsRoles)
        {
            Employee employee = await _employeeRepository.GetByIdAsync(id);
            if(employee == null)
                return NotFound();

            employee.FirstName = fName;
            employee.LastName = lName;
            employee.Email = email;

            IEnumerable<Role> roles = await _rolesRepository.GetAllAsync();
            employee.Roles = roles.Where(r => idsRoles.Contains(r.Id)).ToList();

            return true;
        }
    }
}