using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Preferences;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Контроллер для работы с предпочтениями клиентов.
/// </summary>
public class PreferencesController(IRepository<Preference> preferencesRepository) : BaseController
{
    /// <summary>
    /// Get - возвращает все доступные предпочтения.
    /// Используется клиентскими сценариями, где нужно выбрать интересы клиента.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PreferenceShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PreferenceShortResponse>>> Get(CancellationToken ct)
    {
        var preferences = await preferencesRepository.GetAll(ct: ct);
        return Ok(preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToList());
    }
}
