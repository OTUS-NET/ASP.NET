using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class RolesController : ControllerBase
    {
        private readonly IRepository<Role> _rolesRepository;

        public RolesController(IRepository<Role> rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        /// <summary>
        /// Получить все доступные роли сотрудников
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Получить все доступные роли сотрудников
        /// </summary>
        /// <returns>Список ролей сотрудников</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleItemResponse>>> GetRolesAsync()
        {
            var roles = await _rolesRepository.GetAllAsync();

            var response = roles.Select(x =>
                new RoleItemResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

            return Ok(response);
        }
    }
}