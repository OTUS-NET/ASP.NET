using AutoMapper;
using DirectoryOfPreferences.Application.Abstractions;
using DirectoryOfPreferences.Infrastructure.Extentions;
using DirectoryOfPreferences.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DirectoryOfPreferences.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController(IPreferenceService preferenceService, 
        IMapper mapper, 
        IDistributedCache distributedCache) : ControllerBase
    {
        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<PreferenceResponse>?> GetPreferencesAsync()
        {
            string? serialized = await distributedCache.GetStringAsync(KeyCaching.PreferencesKey(), HttpContext.RequestAborted);
            if (serialized is not null)
            {
                var cachResult = JsonSerializer.Deserialize<IEnumerable<PreferenceResponse>>(serialized);
                if (cachResult is not null)
                    return cachResult;
            }

            var preferences = (await preferenceService.GetAllPreferenceAsync(HttpContext.RequestAborted)).Select(mapper.Map<PreferenceResponse>);

            await distributedCache.SetStringAsync(
                key: KeyCaching.PreferencesKey(),
                value: JsonSerializer.Serialize(preferences),
                options: new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });

            return preferences;
        }

        public async Task<PreferenceResponse> AddPreferencesAsync() 
        {
            
        }
    }
}
