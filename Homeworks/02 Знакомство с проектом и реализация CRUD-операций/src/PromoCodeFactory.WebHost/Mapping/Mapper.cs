using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mapping;

public static class Mapper
{
    public static EmployeeResponse ToEmployeeResponse(Employee employee)
    {
        return new EmployeeResponse(
            employee.Id,
            employee.FullName,
            employee.Email,
            employee.Roles.Select(ToRoleItemResponse).ToList(),
            employee.AppliedPromocodesCount);
    }

    public static EmployeeShortResponse ToEmployeeShortResponse(Employee employee)
    {
        return new EmployeeShortResponse(
            employee.Id,
            employee.FullName,
            employee.Email);
    }

    public static RoleItemResponse ToRoleItemResponse(Role role)
    {
        return new RoleItemResponse(
            role.Id,
            role.Name,
            role.Description);
    }
}
