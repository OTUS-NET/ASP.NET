using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Роли сотрудников
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RolesController(IRepository<Role, Guid> rolesRepository, IMapper mapper)
    {      

        /// <summary>
        /// Получить все доступные роли сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleItemResponse>), 200)]
        public async Task<IEnumerable<RoleItemResponse>> GetRolesAsync()
        {
            var roles = await rolesRepository.GetAllAsync();

            var rolesModelList = roles.Select(mapper.Map<RoleItemResponse>).ToList();

            return rolesModelList;
        }
    }
}