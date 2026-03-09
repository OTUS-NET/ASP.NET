using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models;
using static System.Net.WebRequestMethods;

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
        if (employee is null)
            return this.NotFound(this.GetNotFoundProblemDetails(id));

        return this.Ok(Mapper.ToEmployeeResponse(employee));
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
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Resource not created",
            Type = "https://httpstatuses.com/400",
            Instance = HttpContext.Request.Path
        };

        if (role is null)
        {
            problemDetails.Detail = $"The request contains invalid role Id.";

            return this.BadRequest(problemDetails);
        }

        var employee = Mapper.ToEmployee(request, role);
        try
        {
            await employeeRepository.Add(employee, ct);
        }
        catch (InvalidOperationException ex)
        {
            problemDetails.Detail = ex.Message;

            return BadRequest(problemDetails);
        }
        catch
        {
            problemDetails.Detail = "An unexpected error occurred while creating the employee.";

            return BadRequest(problemDetails);
        }

        var employeeModel = Mapper.ToEmployeeResponse(employee);

        return CreatedAtAction(nameof(GetById), new { id = employeeModel.Id }, employeeModel);
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
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Resource not updated",
            Type = "https://httpstatuses.com/400",
            Instance = HttpContext.Request.Path
        };

        if (role is null)
        {
            problemDetails.Detail = $"The request contains invalid role Id.";

            return this.BadRequest(problemDetails);
        }

        try
        {
            var employee = Mapper.ToEmployee(request, role, id);
            await employeeRepository.Update(employee, ct);

            return Ok(Mapper.ToEmployeeResponse(employee));
        }
        catch (EntityNotFoundException)
        {
            return NotFound(this.GetNotFoundProblemDetails(id));
        }
        catch (InvalidOperationException ex)
        {
            problemDetails.Detail = ex.Message;

            return BadRequest(problemDetails);
        }
        catch
        {
            problemDetails.Detail = "An unexpected error occurred while creating the employee.";

            return BadRequest(problemDetails);
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
        }
        catch (EntityNotFoundException)
        {
            return NotFound(this.GetNotFoundProblemDetails(id));
        }

        return NoContent();
    }

    private ProblemDetails GetNotFoundProblemDetails(Guid id) =>
        new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource not found",
            Detail = $"Employee with Id {id} does not exist.",
            Type = "https://httpstatuses.com/404",
            Instance = HttpContext.Request.Path
        };
}
