using Pcf.Administration.WebHost.Models.Roles;

namespace Pcf.Administration.WebHost.Models.Employees;

public record EmployeeResponse(
    Guid Id,
    string FullName,
    string Email,
    RoleResponse Role);
