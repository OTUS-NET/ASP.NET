using MediatR;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Commands.Queries.Roles;
using PromoCodeFactory.Contracts.Roles;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Роли сотрудников
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class RolesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Получить все доступные роли сотрудников
    /// </summary>
    [HttpGet]
    public Task<List<RoleItemResponse>> GetRolesAsync()
    {
        return _mediator.Send(new GetAllRolesQuery());
    }
}