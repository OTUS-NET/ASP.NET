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
            ToRoleResponse(employee.Role),
            employee.AppliedPromocodesCount);
    }

    public static EmployeeShortResponse ToEmployeeShortResponse(Employee employee)
    {
        return new EmployeeShortResponse(
            employee.Id,
            employee.FullName,
            employee.Email);
    }

    public static RoleResponse ToRoleResponse(Role role)
    {
        return new RoleResponse(
            role.Id,
            role.Name,
            role.Description);
    }

    public static Employee ToEmployee(EmployeeCreateRequest request, Role role)
    {
        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = role,
            AppliedPromocodesCount = 0
        };
    }

    public static Employee ToEmployee(EmployeeUpdateRequest request, Role role, Guid id)
    {
        return new Employee
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = role
        };
    }
}
