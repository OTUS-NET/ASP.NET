using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;

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
        var employee = await employeeRepository.GetById(id, ct);

        return employee != null ? Ok(Mapper.ToEmployeeResponse(employee)) : NotFound();
    }

    /// <summary>
    /// Создать сотрудника
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeResponse>> Create([FromBody] EmployeeCreateRequest request, CancellationToken ct)
    {
        var role = await roleRepository.GetById(request.RoleId, ct);

        if (role != null)
        {
            try
            {
                await employeeRepository.Add(Mapper.ToEmployee(request, role), ct);

                return Created();
            }
            catch (BadHttpRequestException)
            {
                return BadRequest();
            }
        }
        else
            return NotFound();
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

        var employee = await employeeRepository.GetById(id, ct);

        var role = await roleRepository.GetById(request.RoleId, ct);

        if (employee == null || role == null)
            return NotFound();

        try
        {
            await employeeRepository.Update(new Employee()
            {
                Id = employee.Id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = role,
                AppliedPromocodesCount = employee.AppliedPromocodesCount,
            }, ct);

            return Ok();
        }
        catch (BadHttpRequestException)
        {
            return BadRequest();
        }

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

            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

    }
}
