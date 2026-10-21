using Microsoft.AspNetCore.Mvc;
using Pcf.Common.Web.Controllers;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Mapping;
using Pcf.ReceivingFromPartner.WebHost.Models.Preferences;

namespace Pcf.ReceivingFromPartner.WebHost.Controllers;

/// <summary>
/// Предпочтения
/// </summary>
public class PreferencesController(IRepository<Preference> preferencesRepository) : BaseController
{
    /// <summary>
    /// Получить все доступные предпочтения
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PreferenceShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PreferenceShortResponse>>> Get(CancellationToken ct)
    {
        var preferences = await preferencesRepository.GetAll(false, ct);
        return Ok(preferences.Select(PreferencesMapper.ToPreferenceShortResponse));
    }
}
