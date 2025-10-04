using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Pcf.PreferencesDirectory.Core.Abstractions.Repositories;
using Pcf.PreferencesDirectory.Core.Domain;
using Pcf.PreferencesDirectory.WebHost.Models;
using Pcf.SharedLibrary.Models;
using System.Text.Json;

namespace Pcf.PreferencesDirectory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
        : ControllerBase
    {
        private const string _cacheKeyAll = "preferences_all";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IDistributedCache _distributedCache;

        public PreferencesController(IRepository<Preference> preferenceRepository, IDistributedCache distributedCache)
        {
            _preferenceRepository = preferenceRepository;
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            var cached = await _distributedCache.GetStringAsync(_cacheKeyAll);
            if (!string.IsNullOrEmpty(cached))
            {
                var result = JsonSerializer.Deserialize<List<PreferenceResponse>>(cached);
                return Ok(result);
            }

            var preferences = await _preferenceRepository.GetAllAsync();
            var response = preferences.Select(x => new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            await _distributedCache.SetStringAsync(_cacheKeyAll, JsonSerializer.Serialize(response),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheExpiration });

            return Ok(response);
        }

        /// <summary>
        /// Получить предпочтение по id
        /// </summary>
        /// <param name="id">Id предпочтения, например, <example>ef7f299f-92d7-459f-896e-078ed53ef99c</example></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PreferenceResponse>> GetPreferenceAsync(Guid id)
        {
            string cacheKey = $"preference_{id}";
            var cached = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                var result = JsonSerializer.Deserialize<PreferenceResponse>(cached);
                return Ok(result);
            }

            var preference = await _preferenceRepository.GetByIdAsync(id);
            if (preference == null)
                return NotFound();

            var response = new PreferenceResponse { Id = preference.Id, Name = preference.Name };

            var json = JsonSerializer.Serialize(response);
            await _distributedCache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });
            return Ok(response);
        }

        /// <summary>
        /// Создать новое предпочтение
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PreferenceResponse>> CreatePreferenceAsync(CreatePreferenceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Не задано имя предпочтения");

            Preference preference = new() { Id = Guid.NewGuid(), Name = request.Name };
            await _preferenceRepository.AddAsync(preference);

            // Инвалидация кэша
            await _distributedCache.RemoveAsync("preferences_all");

            var response = new PreferenceResponse { Id = preference.Id, Name = preference.Name };
            return CreatedAtAction(nameof(GetPreferenceAsync), new { id = preference.Id }, response);
        }

        /// <summary>
        /// Удалить предпочтение
        /// </summary>
        /// <param name="id">Id предпочтения, например, <example>ef7f299f-92d7-459f-896e-078ed53ef99c</example></param>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePreferenceAsync(Guid id)
        {
            var preference = await _preferenceRepository.GetByIdAsync(id);
            if (preference == null)
                return NotFound();

            await _preferenceRepository.DeleteAsync(preference);

            // Инвалидация кэша
            await _distributedCache.RemoveAsync("preferences_all");
            await _distributedCache.RemoveAsync($"preference_{id}");

            return NoContent();
        }

    }
}
