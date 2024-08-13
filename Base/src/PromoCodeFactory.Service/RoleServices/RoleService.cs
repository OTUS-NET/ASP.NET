using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Service.Roles;
using PromoCodeFactory.Service.RoleServices;
using PromoCodeFactory.Service.RoleServices.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Service.RoleServices
{
    public class RoleService : IRoleService
    {
        protected IRepository<Role> _rolesRepository; 

        public RoleService(IRepository<Role> rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }   

        public async Task<IEnumerable<RoleItemResponse>> GetRolesAsync()
        {
            var roles = await _rolesRepository.GetAllAsync();

            var rolesModelList = roles.Select(x =>
                new RoleItemResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

            return rolesModelList;
        }

        public async Task<IEnumerable<Role>> GetRolesFromIdsAsync(IEnumerable<Guid> roleIds)
        {
            var roles = await _rolesRepository.GetByIdsAsync(roleIds);

            return roles;
        }
    }
}
