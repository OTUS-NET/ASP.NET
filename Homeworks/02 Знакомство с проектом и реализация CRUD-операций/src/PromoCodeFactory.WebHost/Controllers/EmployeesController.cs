using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using PromoCodeFactory.Core.Abstractions.Services;
using PromoCodeFactory.Core.Services;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Сотрудники
/// </summary>
public class EmployeesController(
    IRepository<Employee> employeeRepository,
    IEmployeesService employeesService,
    IRoleService roleService
    ) : BaseController
{
    /// <summary>
    /// Получить данные всех сотрудников
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmployeeShortResponse>>> Get(CancellationToken ct)
    {
        var employees = await employeeRepository.GetAll(ct);

        var employeesModels = employees.Select(Mapper.ToEmployeeShortResponse).ToList();

        return Ok(employeesModels);
    }

    /// <summary>
    /// Получить данные сотрудника по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var employee = await employeesService.GetById(id, ct);

        if (employee == null)
            return NotFound();

        var employeesModel = Mapper.ToEmployeeResponse(employee);      
        return Ok(employeesModel);
    }

    /// <summary>
    /// Создать сотрудника
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeResponse>> Create([FromBody] EmployeeCreateRequest request, CancellationToken ct)
    {
        var idRole = await roleService.GetById(request.RoleId, ct);

        if (idRole == null)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid role",
                Detail = $"Role with id {request.RoleId} not found",
                Status = StatusCodes.Status400BadRequest
            });


        var employee = Mapper.ToEmployee(request, idRole);
        await employeesService.CreateEmployee(employee, ct);
        var employeesModel = Mapper.ToEmployeeResponse(employee);
        return Ok(employeesModel);
    }

    /// <summary>
    /// Обновить сотрудника
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] EmployeeUpdateRequest request,
        CancellationToken ct)
    {
        var employee = await employeesService.GetById(id, ct);

        if (employee == null)
            return NotFound();

        var idRole = await roleService.GetById(request.RoleId, ct);
        
        if (idRole == null)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid role",
                Detail = $"Role with id {request.RoleId} not found",
                Status = StatusCodes.Status400BadRequest
            });
        employee = Mapper.ToEmployee(request, id, idRole);
        var result = employeesService.UpdateEmployee(employee, ct);
        return Ok(result);
    }

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
