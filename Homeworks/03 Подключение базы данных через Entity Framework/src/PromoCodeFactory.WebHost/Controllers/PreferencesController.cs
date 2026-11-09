using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Preferences;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтения
/// </summary>
public class PreferencesController : BaseController
{
    private IRepository<Preference> _preferenceRepository;

    public PreferencesController(IRepository<Preference> preferenceRepository)
    {
        _preferenceRepository = preferenceRepository;
    }

    /// <summary>
    /// Получить все доступные предпочтения
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PreferenceShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PreferenceShortResponse>>> Get(CancellationToken ct)
    {
        var data = (await _preferenceRepository.GetAll(true))
            .Select(p => new PreferenceShortResponse(p.Id, p.Name)).ToList();
        return Ok(data);
    }
}
