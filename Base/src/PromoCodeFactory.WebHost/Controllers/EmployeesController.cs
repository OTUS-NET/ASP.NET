using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Contracts.Employees;
using PromoCodeFactory.Core.Administration;
using PromoCodeFactory.DataAccess.Repositories;

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
    [HttpGet("[action]")]
    public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();

        var employeesModelList = employees.Select(x =>
            new EmployeeShortResponse
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
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployeeByIdAsync([FromRoute] Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return NotFound();

        var employeeModel = new EmployeeResponseDto()
        {
            Id = employee.Id,
            Email = employee.Email,
            FullName = employee.FullName,
            AppliedPromocodesCount = employee.AppliedPromoCodesCount
        };

        return employeeModel;
    }

    /// <summary>
    /// Создать сотрудника
    /// </summary>
    [HttpPost("[action]")]
    public async Task<ActionResult> Create([FromBody] EmployeeSetDto data)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Email = data.Email,
            FirstName = data.FirstName,
            LastName = data.LastName,
        };

        if (data.Role is not null)
        {
        }

        await _employeeRepository.CreateAsync(employee);

        return Ok();
    }

    /// <summary>
    /// Обновить данные сотрудника
    /// </summary>
    /// <returns></returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> Update([FromRoute] Guid id, EmployeeSetDto data)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee is null)
        {
            return NotFound("Employee not found");
        }

        employee.Email = data.Email;
        employee.FirstName = data.FirstName;
        employee.LastName = data.LastName;

        await _employeeRepository.UpdateAsync(employee);

        return Ok();
    }

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee is null)
        {
            return NotFound("Employee not found");
        }

        await _employeeRepository.DeleteAsync(employee.Id);

        return Ok();
    }
}