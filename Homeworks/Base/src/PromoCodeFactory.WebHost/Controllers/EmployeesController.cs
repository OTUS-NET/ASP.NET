using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Сотрудники
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController(IRepository<Employee> employeeRepository) : ControllerBase
{
    private readonly IRepository<Employee> _employeeRepository = employeeRepository;

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
    /// Удалить данные сотрудника по Id
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteEmployeeByIdAsync(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        await _employeeRepository.DeleteEntityAsync(employee);
        return true;
    }

    /// <summary>
    /// Обновить данные сотрудника
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeEmailAsync([FromQuery] EmployeeUpdateModel employee)
    {
        var newEmployee = await _employeeRepository.GetByIdAsync(employee.Id);

        newEmployee.FirstName = employee.FirstName;
        newEmployee.LastName = employee.LastName;
        newEmployee.AppliedPromocodesCount = employee.AppliedPromocodesCount;
        newEmployee.Email = employee.Email;    

        return CreateEmployeeResponse(newEmployee);
    }

    /// <summary>
    /// Добавить данные сотрудника
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> AddEmployeeAsync([FromQuery] EmployeeCreateModel employee)
    {
        Guid id = Guid.NewGuid();

        await _employeeRepository.CreateEntityAsync(new Employee
        {
            Id = id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            AppliedPromocodesCount = employee.AppliedPromocodesCount
        });

        var newEmployee = await _employeeRepository.GetByIdAsync(id);

        return CreateEmployeeResponse(newEmployee);
    }

    private static EmployeeResponse CreateEmployeeResponse(Employee newEmployee)
    {
        return new EmployeeResponse()
        {
            Id = newEmployee.Id,
            Roles = newEmployee.Roles?.Select(x => new RoleItemResponse()
            {
                Name = x.Name,
                Description = x.Description
            }).ToList() ?? [],
            AppliedPromocodesCount = newEmployee.AppliedPromocodesCount,            
            FullName = newEmployee.FullName,
            Email = newEmployee.Email,
        };
    }
}