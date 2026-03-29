using PromoCodeFactory.WebHost.Models.Roles;

namespace PromoCodeFactory.WebHost.Models.Employees;

public record EmployeeResponse(
    Guid Id,
    string FullName,
    string Email,
    RoleResponse Role);
