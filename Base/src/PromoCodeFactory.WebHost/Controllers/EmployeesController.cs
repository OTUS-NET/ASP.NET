using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Employee> employees = await _employeeRepository.GetAllAsync(cancellationToken);

            List<EmployeeShortResponse> employeesModelList = employees.Select(x => new EmployeeShortResponse()
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
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Employee employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);

            if (employee == null)
                return NotFound();

            EmployeeResponse employeeModel = new EmployeeResponse()
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
        [HttpPost("Create")]
        public async Task<ActionResult<Guid>> Create(EmployeeRequest employeeRequest, CancellationToken cancellationToken = default)
        {
            Employee employee = await _employeeRepository.CreateAsync(cancellationToken);
            await FillEmployeeFromRequest(employee, employeeRequest, cancellationToken);
            await _employeeRepository.AddAsync(employee, cancellationToken);
            return Ok(employee.Id);
        }

        /// <summary>
        /// Удаление работника
        /// </summary>
        [HttpGet("Delete/id:{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            if((await _employeeRepository.GetAllAsync()).Any())
                return NoContent();

            bool result = await _employeeRepository.RemoveByIdAsync(id, cancellationToken);
            if(!result)
                return NotFound($"Data not found id={id}");
            return Ok(result);
        }

        /// <summary>
        /// Обновление данных работника
        /// </summary>
        [HttpPost("Update/id:{id}")]
        public async Task<ActionResult<EmployeeShortResponse>> Update(Guid id, EmployeeRequest employeeRequest, CancellationToken cancellationToken = default)
        {
            Employee employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if(employee == null)
                return NotFound($"Data not found id={id}");

            await FillEmployeeFromRequest(employee, employeeRequest, cancellationToken);
            Employee newRecord = await _employeeRepository.UpdateAsync(employee, cancellationToken);
            return Ok(ToEmployeeShortResponse(newRecord));
        }

        private async Task FillEmployeeFromRequest(Employee employee, EmployeeRequest employeeRequest, CancellationToken cancellationToken = default)
        {
            employee.FirstName = employeeRequest.FirstName;
            employee.LastName = employeeRequest.LastName;
            employee.Email = employeeRequest.Email;

            IEnumerable<Role> roles = await _rolesRepository.GetAllAsync(cancellationToken);
            employee.Roles = roles.Where(r => employeeRequest.Roles?.Select(requestR => requestR.Id)
                                                                    .Contains(r.Id) ?? false)
                                  .ToList();
        }

        private EmployeeShortResponse ToEmployeeShortResponse(Employee employee)
        {
            EmployeeShortResponse response = new EmployeeShortResponse();
            response.Id = employee.Id;
            response.FullName = employee.FullName;
            response.Email = employee.Email;
            return response;
        }
    }
}