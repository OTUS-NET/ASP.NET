using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтение
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PreferencesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRepository<Preference> _preferenceRepository;

    public PreferencesController(IRepository<Preference> preferenceRepository, IMapper mapper)
    {
        _mapper = mapper;
        _preferenceRepository = preferenceRepository;
    }

    /// <summary>
    /// Получить все предпочтения
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<PreferenceShortResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var customers = (await _preferenceRepository.GetAllAsync(cancellationToken))
            .Select(c => _mapper.Map<PreferenceShortResponse>(c)).ToList();
        return Ok(customers);
    }
}