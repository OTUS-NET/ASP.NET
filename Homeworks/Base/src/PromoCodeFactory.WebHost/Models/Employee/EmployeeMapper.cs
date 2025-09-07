using System.Linq;
using PromoCodeFactory.WebHost.Models.RoleItem;

namespace PromoCodeFactory.WebHost.Models.Employee;

using Core.Domain.Administration;

/// <summary>
/// DTO mapper
/// </summary>
public class EmployeeMapper
{
    public static Employee ToEntity(EmployeeAddRequest request) =>
        new Employee()
        {
            LastName = request.LastName,
            FirstName = request.FirstName,
            Email = request.Email
        };
    
    public static EmployeeResponse ToResponse(Employee employee) =>
        new EmployeeResponse()
        {
            Id = employee.Id,
            Email = employee.Email,
            Roles = employee.Roles?.Select(x => new RoleItemResponse()
            {
                Name = x.Name,
                Description = x.Description
            }).ToList(),
            FullName = employee.FullName,
            AppliedPromocodesCount = employee.AppliedPromocodesCount
        };

    public static EmployeeShortResponse ToShortResponse(Employee employee) =>
        new EmployeeShortResponse()
        {
            Id = employee.Id,
            Email = employee.Email,
            FullName = employee.FullName,
        };
}