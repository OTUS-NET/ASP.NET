using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EmployeeRepository : InMemoryRepositoryBase<Employee>
    {
        private readonly IRepository<Role> _roleRepository;
        public EmployeeRepository(
            IDictionary<Guid, Employee> data,
            IRepository<Role> roleRepository) : base(data)
        {
            _roleRepository = roleRepository;
        }

        public override async Task<Employee> CreateAsync(Employee entity)
        {
            if (!await ValidateRolesExistAsync(entity.Roles))
                return null;
            return await base.CreateAsync(entity);
        }

        public override async Task<Employee> UpdateAsync(Employee entity)
        {
            if (!await ValidateRolesExistAsync(entity.Roles))
                return null;
            return await base.UpdateAsync(entity);
        }

        /// <summary>
        /// Проверить, что такие роли существуют
        /// </summary>
        /// <param name="roleGuids"></param>
        /// <returns></returns>
        private async Task<bool> ValidateRolesExistAsync(IEnumerable<Guid> roleGuids)
        {
            var roles = await _roleRepository.GetAllAsync();
            foreach (var roleId in roleGuids)
            {
                if (roles.FirstOrDefault(x => x.Id == roleId) == null)
                    return false;
            }
            return true;
        }
    }
}
