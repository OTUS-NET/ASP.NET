using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models.Roles;

namespace PromoCodeFactory.WebHost.Models.Employees
{
    public static class EmployeeDtoExtensions
    {
        public static Employee ToEntity(this EmployeeRequest dto)
        {
            var result = new Employee();
            result.Id = dto.Id;
            result.FirstName = dto.FirstName;
            result.LastName = dto.LastName;
            result.Email = dto.Email;
            result.AppliedPromocodesCount = dto.AppliedPromocodesCount;
            result.Roles = new List<Guid>(dto.Roles);
            return result;
        }

        public static async Task<EmployeeResponse> ToDtoAsync(this Employee entity, IRepository<Role> roleRepository)
        {
            var result = new EmployeeResponse();
            result.Id = entity.Id;
            result.FullName = entity.FullName;
            result.Email = entity.Email;
            result.AppliedPromocodesCount = entity.AppliedPromocodesCount;
            result.Roles = new List<RoleDto>(
                (await roleRepository.GetAllAsync())
                .Where(x => entity.Roles.Contains(x.Id))
                .Select(x => x.ToDto())
                );

            return result;
        }

    }
}
