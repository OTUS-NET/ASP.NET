using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Service.RoleServices;
using PromoCodeFactory.Service.RoleServices.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.Service.Roles
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleItemResponse>> GetRolesAsync();

        Task<IEnumerable<Role>> GetRolesFromIdsAsync(IEnumerable<Guid> roleIds);
    }
}
