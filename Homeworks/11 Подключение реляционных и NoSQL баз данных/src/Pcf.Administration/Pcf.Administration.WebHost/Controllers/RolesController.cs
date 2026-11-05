using Microsoft.AspNetCore.Mvc;
using Pcf.Administration.Core.Domain;
using Pcf.Administration.WebHost.Mapping;
using Pcf.Administration.WebHost.Models.Roles;
using Pcf.Common.Web.Controllers;

namespace Pcf.Administration.WebHost.Controllers;

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
