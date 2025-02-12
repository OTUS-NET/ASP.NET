using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Services.Abstractions;
using PromoCodeFactory.WebHost.Models.Preference;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтение
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PreferencesController : ControllerBase
{
    private readonly IPreferenceService _preferenceService;
    private readonly IMapper _mapper;

    public PreferencesController(IPreferenceService preferenceService, IMapper mapper)
    {
        _preferenceService = preferenceService;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Получить все предпочтения
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PreferenceShortResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var customers = (await _preferenceService.GetAllAsync(cancellationToken))
            .Select(c => _mapper.Map<PreferenceShortResponse>(c)).ToList();

        return Ok(customers);
    }
}