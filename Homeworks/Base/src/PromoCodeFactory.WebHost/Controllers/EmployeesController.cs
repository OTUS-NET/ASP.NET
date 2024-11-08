using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Mappers;
using PromoCodeFactory.WebHost.Models;

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

        return employee.Map();
    }

    /// <summary>
    /// Добавить сотрудника
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> AddEmployeeAsync(AddEmployeeRequest request)
    {
        if (request == null)
            return BadRequest($"{nameof(AddEmployeeRequest)} cannot be null");

        var employee = request.Map();

        employee = await _employeeRepository.CreateAsync(employee);

        return employee.Map();
    }

    /// <summary>
    ///Обновить данные сотрудника
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(EmployeeRequest request)
    {
        if (request == null)
            return BadRequest($"{nameof(EmployeeRequest)} cannot be null");

        if (request.Id == Guid.Empty)
            return BadRequest($"Id cannot be empty");

        var employee = request.Map();

        employee = await _employeeRepository.UpdateAsync(employee);

        return employee.Map();
    }

    /// <summary>
    /// Удалить запись о сотруднике
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteEmployeeByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest($"Id cannot be empty");

        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return NotFound();

        return await _employeeRepository.DeleteAsync(id);
    }
}