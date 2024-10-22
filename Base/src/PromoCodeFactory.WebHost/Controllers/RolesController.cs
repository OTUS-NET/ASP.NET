using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Contracts.Roles;
using PromoCodeFactory.Core.Administration;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Роли сотрудников
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class RolesController(IRepository<Role> rolesRepository)
{
    private readonly IRepository<Role> _rolesRepository = rolesRepository;

    /// <summary>
    /// Получить все доступные роли сотрудников
    /// </summary>
    [HttpGet]
    public async Task<List<RoleItemResponse>> GetRolesAsync()
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
}