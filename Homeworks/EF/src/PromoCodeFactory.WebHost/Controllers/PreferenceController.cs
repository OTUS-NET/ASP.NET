using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Preference
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferenceController : ControllerBase
    {
        private readonly IRepository<Preference> _repository;

        public PreferenceController(IRepository<Preference> repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// Получения всех Preference
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            var preferences = await _repository.GetAllAsync();

            //response

            var preferencesResponseList = preferences.Select(x =>
                new PreferenceResponse()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            return preferencesResponseList;
        }
    }
}
