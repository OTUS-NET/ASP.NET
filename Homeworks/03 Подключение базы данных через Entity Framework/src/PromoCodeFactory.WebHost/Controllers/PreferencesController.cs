using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.Preferences;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтения
/// </summary>
public class PreferencesController(IRepository<Preference> preferenceRepository) : BaseController
{
    /// <summary>
    /// Получить все доступные предпочтения
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PreferenceShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PreferenceShortResponse>>> Get(CancellationToken ct)
    {
        var preferences = await preferenceRepository.GetAll(withIncludes: false, ct);

        var response = preferences.Select(p => new PreferenceShortResponse(
            p.Id,
            p.Name)).ToList();

        return Ok(response);
    }
}
