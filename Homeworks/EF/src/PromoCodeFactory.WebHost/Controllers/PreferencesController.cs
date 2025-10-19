using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PreferencesController(IRepository<Preference> preferenceRepository)
    : ControllerBase
{
    /// <summary>
    /// Получить список предпочтений
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Preference>> GetPreferencesAsync()
    {
        var result = await preferenceRepository.GetAllAsync();

        return Ok(result);
    }
}