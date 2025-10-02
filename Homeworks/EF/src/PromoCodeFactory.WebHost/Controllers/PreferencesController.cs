using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController(
        ILogger<PreferencesController> logger,
        IRepository<Preference> preferencesRepository
    ) : ControllerBase
    {
        /// <summary>
        /// Получить все предпочтения
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            try
            {
                var codes = await preferencesRepository.GetAllAsync();

                return Ok(codes.Select(x => new PreferenceResponse()
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToList());
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error get preferences list: [{msg}]", e.Message);
                return StatusCode(500, e.Message);
            }
        }
    }
}