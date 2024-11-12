using MediatR;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Commands.Queries.Preferences;
using PromoCodeFactory.Contracts.Preferences;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтения клиентов
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PreferencesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить список всех предпочтений
    /// </summary>
    [HttpGet]
    public Task<List<PreferenceResponseDto>> GetAllPreferences()
    {
        return mediator.Send(new GetAllPreferencesQuery());
    }
}