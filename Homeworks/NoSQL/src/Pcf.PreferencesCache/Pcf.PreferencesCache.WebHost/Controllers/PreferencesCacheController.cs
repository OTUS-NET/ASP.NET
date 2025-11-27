using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pcf.PreferencesCache.WebHost.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pcf.PreferencesCache.WebHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PreferencesCacheController : ControllerBase
    {
        private readonly ILogger<PreferencesCacheController> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        private IDatabase GetDatabase() => _redis.GetDatabase();

        public PreferencesCacheController(ILogger<PreferencesCacheController> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
        }

        /// <summary>
        /// Получить все предпочтения
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Preference>>> GetAllAsync()
        {
            var db = GetDatabase();
            string cacheKey = "preferences:all";

            var cached = await db.StringGetAsync(cacheKey);

            if (!cached.IsNull)
            {
                return Ok(JsonSerializer.Deserialize<List<Preference>>(cached));
            }

            return NotFound();
        }

        /// <summary>
        /// Получить предпочтение
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Preference>> GetByIdAsync(Guid id)
        {
            var db = GetDatabase();
            string cacheKey = $"preference:{id}";

            var cached = await db.StringGetAsync(cacheKey);

            if (!cached.IsNull)
            {
                return Ok(JsonSerializer.Deserialize<Preference>(cached));
            }

            return NotFound();
        }

        /// <summary>
        /// Добавить предпочтение в кэш
        /// </summary>
        /// <param name="preference"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddPreferenceAsync([FromBody] Preference preference)
        {
            var db = GetDatabase();
            string cacheKey = $"preference:{preference.Id}";

            await db.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(preference),
                _cacheExpiration);

            return Ok();
        }

        /// <summary>
        /// Добавить список предпочтений в кеш
        /// </summary>
        /// <param name="preferences"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult> AddPreferencesAsync([FromBody] IEnumerable<Preference> preferences)
        {
            var db = GetDatabase();
            string cacheKey = $"preferences:all";

            await db.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(preferences),
                _cacheExpiration);

            return Ok();
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> InvalidateCacheAsync(Guid id)
        {
            var db = GetDatabase();
            await db.KeyDeleteAsync($"preference:{id}");
            return Ok();
        }

        [HttpPut("all")]
        public async Task<ActionResult> InvalidateAllCacheAsync()
        {
            var db = GetDatabase();
            await db.KeyDeleteAsync("preferences:all");
            return Ok();
        }
    }
}
