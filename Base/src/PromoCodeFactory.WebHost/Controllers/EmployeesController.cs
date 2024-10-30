using MediatR;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Commands.Commands.Employees;
using PromoCodeFactory.Commands.Queries.Employees;
using PromoCodeFactory.Contracts;
using PromoCodeFactory.Contracts.Employees;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Сотрудники
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Получить данные всех сотрудников
    /// </summary>
    [HttpGet("[action]")]
    public Task<List<EmployeeShortResponse>> GetEmployeesAsync()
    {
        return _mediator.Send(new GetAllEmployeesQuery());
    }

    /// <summary>
    /// Получить данные сотрудника по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public Task<EmployeeResponseDto> GetEmployeeByIdAsync([FromRoute] Guid id)
    {
        return _mediator.Send(new GetEmployeeByIdQuery
        {
            Id = id
        });
    }

    /// <summary>
    /// Создать сотрудника
    /// </summary>
    [HttpPost("[action]")]
    public Task<ResponseId<Guid>> Create([FromBody] EmployeeSetDto data)
    {
        return _mediator.Send(new CreateEmployeeCommand
        {
            Data = data,
        });
    }

    /// <summary>
    /// Обновить данные сотрудника
    /// </summary>
    [HttpPatch("{id:guid}")]
    public Task Update([FromRoute] Guid id, EmployeeSetDto data)
    {
        return _mediator.Send(new UpdateEmployeeCommand
        {
            Id = id,
            Data = data
        });
    }

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    [HttpDelete("{id:guid}")]
    public Task Delete([FromRoute] Guid id)
    {
        return _mediator.Send(new DeleteEmployeeCommand
        {
            Id = id
        });
    }
}