using Pcf.Administration.Core.Domain;
using Pcf.Administration.WebHost.Models.Employees;

namespace Pcf.Administration.WebHost.Mapping;

public static class EmployeesMapper
{
    public static EmployeeResponse ToEmployeeResponse(Employee employee)
    {
        return new EmployeeResponse(
            employee.Id,
            employee.FullName,
            employee.Email,
            RolesMapper.ToRoleResponse(employee.Role));
    }

    public static EmployeeShortResponse ToEmployeeShortResponse(Employee employee)
    {
        return new EmployeeShortResponse(
            employee.Id,
            employee.FullName,
            employee.Email);
    }

    public static Employee ToEmployee(EmployeeCreateRequest request, Role role)
    {
        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = role
        };
    }
}
