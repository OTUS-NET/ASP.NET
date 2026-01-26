using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Роли сотрудников
/// </summary>
public class RolesController(IRepository<Role> rolesRepository) : BaseController
{
    /// <summary>
    /// Получить все доступные роли сотрудников
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoleItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoleItemResponse>>> Get(CancellationToken ct)
    {
        var roles = await rolesRepository.GetAll(ct);

        var rolesModels = roles.Select(Mapper.ToRoleItemResponse).ToList();

        return Ok(rolesModels);
    }
}
