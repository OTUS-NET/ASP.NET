using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
        : ControllerBase
    {
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IPreferenceCacheGateway _preferenceCacheGateway;

        public PreferencesController(IRepository<Preference> preferencesRepository, IPreferenceCacheGateway preferenceCacheGateway)
        {
            _preferencesRepository = preferencesRepository;
            _preferenceCacheGateway = preferenceCacheGateway;
        }

        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            try
            {
                var preferencesFromCache = await _preferenceCacheGateway.GetAllAsync();

                if (preferencesFromCache != null)
                {
                    return Ok(preferencesFromCache.Select(x => new PreferenceResponse()
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList());
                }
            }
            catch (Exception)
            {

            }

            var preferences = await _preferencesRepository.GetAllAsync();

            var response = preferences.Select(x => new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            try
            {
                await _preferenceCacheGateway.AddPreferencesAsync(preferences);
            }
            catch (Exception) { }
            return Ok(response);
        }
    }
}