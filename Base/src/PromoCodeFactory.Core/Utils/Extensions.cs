using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Utils
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
    }
}
