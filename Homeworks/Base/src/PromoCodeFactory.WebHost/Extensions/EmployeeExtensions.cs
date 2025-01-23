using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.Core.Extensions
{
    public static class EmployeeExtensions
    {
        public static Employee ToEntity(this CreateEmployeeRequest request, List<Role> roles)
        {
            Employee entity = new()
            {
                FirstName = request.Firstname,
                LastName = request.Lastname,
                Email = request.Email,
                Roles = roles,
                AppliedPromocodesCount = request.AppliedPromocodesCount
            };

            return entity;
        }

        public static Employee ToEntity(this EditEmployeeRequest request, List<Role> roles)
        {
            Employee entity = new()
            {
                Id = request.Id,
                FirstName = request.Firstname,
                LastName = request.Lastname,
                Email = request.Email,
                Roles = roles,
                AppliedPromocodesCount = request.AppliedPromocodesCount
            };

            return entity;
        }

        public static EmployeeShortResponse ToShortResponse(this Employee entity)
        {
            return new EmployeeShortResponse()
            {
                Id = entity.Id,
                Email = entity.Email,
                FullName = entity.FullName,
            };
        }
        public static EmployeeResponse ToResponse(this Employee entity)
        {
            return new EmployeeResponse()
            {
                Id = entity.Id,
                Email = entity.Email,
                Roles = entity.Roles.Select(x => x.ToResponse()).ToList(),
                FullName = entity.FullName,
                AppliedPromocodesCount = entity.AppliedPromocodesCount
            };
        }
    }
}
