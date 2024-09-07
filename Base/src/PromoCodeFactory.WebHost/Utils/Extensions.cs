using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost.Utils
{
    public static class Extensions
    {
        public static Employee ToEmployee(this EmployeeDto dto, Guid? id = null) =>
            new Employee { 
                Id = id ?? Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Roles = dto.Roles,
                AppliedPromocodesCount = dto.AppliedPromocodesCount,
            };

        public static EmployeeResponse ToEmployeeResponse(this Employee employee) =>
            new EmployeeResponse
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

        public static IList<EmployeeShortResponse> ToEmployeeShortResponse(this IList<Employee> employees) =>
            employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();
    }
}
