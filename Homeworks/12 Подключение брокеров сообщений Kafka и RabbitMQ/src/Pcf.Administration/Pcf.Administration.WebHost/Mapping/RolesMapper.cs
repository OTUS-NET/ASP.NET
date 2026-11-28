using Pcf.Administration.Core.Domain;
using Pcf.Administration.WebHost.Models.Roles;

namespace Pcf.Administration.WebHost.Mapping;

public static class RolesMapper
{
    public static RoleResponse ToRoleResponse(Role role)
    {
        return new RoleResponse(
            role.Id,
            role.Name,
            role.Description);
    }
}
