using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Роли сотрудников
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RolesController(
        ILogger<RolesController> logger,
        IRepository<Role> rolesRepository
        ) : ControllerBase
    {
        /// <summary>
        /// Получить все доступные роли сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleItemResponse>>> GetRolesAsync()
        {
            try
            {
               var roles = await rolesRepository.GetAllAsync();

                var rolesModelList = roles.Select(x =>
                    new RoleItemResponse()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    }).ToList();

                return rolesModelList;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error get roles list: [{msg}]", e.Message);
                return StatusCode(500, e.Message);
            }
        }
    }
}