using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Сотрудники
/// </summary>
public class EmployeesController(
    IRepository<Employee> employeeRepository,
    IRepository<Role> roleRepository
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
        var employe = await employeeRepository.GetById(id, ct);

        if (employe == null) return NotFound();

        var employeeModel = Mapper.ToEmployeeResponse(employe);

        return Ok(employeeModel);
    }

    /// <summary>
    /// Создать сотрудника
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeResponse>> Create([FromBody] EmployeeCreateRequest request,
        CancellationToken ct)
    {
        var role = await roleRepository.GetById(request.RoleId, ct);

        if (role == null)
            return Problem(
                title: "Create Employee canceled.",
                detail: $"Role {request.RoleId} does not exist",
                statusCode: StatusCodes.Status400BadRequest);

        var newEntity = Mapper.ToEmployee(request, role);

        await employeeRepository.Add(newEntity, ct);

        var employeeModel = Mapper.ToEmployeeResponse(newEntity);

        return CreatedAtAction(nameof(GetById), new { id = newEntity.Id }, employeeModel);
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
        var role = await roleRepository.GetById(request.RoleId, ct);

        if (role == null)
            return Problem(
                title: "Update Employee canceled.",
                detail: $"Role {request.RoleId} does not exist",
                statusCode: StatusCodes.Status400BadRequest);

        var updatedEntity = new Employee
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = role
        };

        try
        {
            await employeeRepository.Update(updatedEntity, ct);
        }
        catch (EntityNotFoundException<Employee>)
        {
            return Problem(
                title: "Update Employee canceled.",
                detail: $"Employee Id {id} does not exist",
                statusCode: StatusCodes.Status404NotFound);
        }

        var employeeModel = Mapper.ToEmployeeResponse(updatedEntity);

        return Ok(employeeModel);
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
        try
        {
            await employeeRepository.Delete(id, ct);
        }
        catch (EntityNotFoundException<Employee>)
        {
            return Problem(
                title: "Delete Employee canceled.",
                detail: $"Employee Id {id} does not exist",
                statusCode: StatusCodes.Status404NotFound);
        }

        return NoContent();
    }
}
