using Microsoft.AspNetCore.Mvc;
using Pcf.PreferencesCache.WebHost.Models;
using StackExchange.Redis;
using System.Text.Json;

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

        [HttpGet]
        public async Task<ActionResult<List<Preference>>> GetAllAsync()
        {
            var db = GetDatabase();
            string cacheKey = "preferences:all";

            var cached = await db.StringGetAsync(cacheKey);

            if (!cached.IsNull)
            {
                return Ok(JsonSerializer.Deserialize<IEnumerable<Preference>>(cached));
            }

            return NotFound();
        }

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
