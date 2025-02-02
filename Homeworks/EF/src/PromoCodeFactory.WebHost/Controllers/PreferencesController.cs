using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Extensions;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly IRepository<Preference> _preferenceRepository;

        public PreferencesController(IRepository<Preference> preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Получить список всех предпочтений
        /// </summary>
        /// <returns>Список предпочтений</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreferenceResponse>>> GetPreferencesAsync()
        {
            var preferences = await _preferenceRepository.GetAllAsync();
            var response = preferences.Select(p => p.ToResponse()).ToList();
            return Ok(response);
        }
    }
}