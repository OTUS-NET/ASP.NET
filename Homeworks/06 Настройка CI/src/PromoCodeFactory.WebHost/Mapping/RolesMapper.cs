using PromoCodeFactory.WebHost.Models.Roles;

namespace PromoCodeFactory.WebHost.Mapping;

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
