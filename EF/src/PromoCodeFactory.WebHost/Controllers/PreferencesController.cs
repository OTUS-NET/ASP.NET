using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly IRepository<Preference> _preferencesRepository;

        public PreferencesController(IRepository<Preference> preferencesRepository) => _preferencesRepository = preferencesRepository;

        /// <summary>
        /// Получить все предпочтения 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync(CancellationToken cancellationToken = default)
        {
            var preferences = await _preferencesRepository.GetAllAsync(cancellationToken);

            var preferencesResponse = preferences.Select(x => new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return Ok(preferencesResponse);
        }

        /// <summary>
        /// Добавить предпочтение
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PreferenceResponse>> AddPreferenceAsync(CreatePreferenceRequest request, CancellationToken cancellationToken = default)
        {
            var preference = new Preference()
            {
                Name = request.Name,
            };

            var newPreference = await _preferencesRepository.CreateAsync(preference, cancellationToken);

            return Ok(new PreferenceResponse { Id = newPreference.Id, Name = newPreference.Name});
        }
    }
}