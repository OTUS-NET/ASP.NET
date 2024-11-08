using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Linq;

namespace PromoCodeFactory.WebHost.Mappers;

public static class EmployeeMapper
{
    public static EmployeeResponse Map(this Employee employee)
    {
        return new EmployeeResponse()
        {
            Id = employee.Id,
            Email = employee.Email,
            Roles = employee.Roles.Select(x => new RoleItemResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList(),
            FullName = employee.FullName,
            AppliedPromocodesCount = employee.AppliedPromocodesCount
        };
    }

    public static Employee Map(this AddEmployeeRequest request)
    {
        return new Employee()
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Roles = request.Roles.Select(x => new Role()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList(),
            AppliedPromocodesCount = request.AppliedPromocodesCount
        };
    }

    public static Employee Map(this EmployeeRequest request)
    {
        return new Employee()
        {
            Id = request.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Roles = request.Roles.Select(x => new Role()
            {
                Name = x.Name,
                Description = x.Description
            }).ToList(),
            AppliedPromocodesCount = request.AppliedPromocodesCount
        };
    }
}
