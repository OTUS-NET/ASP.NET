using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Service.Roles;
using PromoCodeFactory.Service.RoleServices.ViewModel;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Роли сотрудников
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RolesController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Получить все доступные роли сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<RoleItemResponse>> GetRolesAsync()
        {   
            return await _roleService.GetRolesAsync();
        }
    }
}