using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Roles;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Роли сотрудников
/// </summary>
public class RolesController(IRepository<Role> rolesRepository) : BaseController
{
    /// <summary>
    /// Получить все доступные роли сотрудников
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoleResponse>>> Get(CancellationToken ct)
    {
        var roles = await rolesRepository.GetAll(ct: ct);

        var rolesModels = roles.Select(RolesMapper.ToRoleResponse).ToList();

        return Ok(rolesModels);
    }
}
