using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Sevices;
using PromoCodeFactory.WebHost.Models.Preferences;
using PromoCodeFactory.WebHost.Mapping;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтения
/// </summary>
public class PreferencesController(IPreferencesService _preferencesService) : BaseController
{
    /// <summary>
    /// Получить все доступные предпочтения
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PreferenceShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PreferenceShortResponse>>> Get(CancellationToken ct)
    {
        var result = await _preferencesService.GetAsync(true, ct);
        var response = result.Select(x => x.ToPreferenceShortResponse());
        return Ok(result);
    }
}
