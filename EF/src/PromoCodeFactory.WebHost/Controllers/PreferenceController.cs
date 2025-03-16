using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.WebHost.Models.Response;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения покупателей.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferenceController(IRepository<Preference, Guid> preferenceRepository, IMapper mapper) : ControllerBase
    {
        /// <summary>
        ///  Получить все предпочтения клиентов.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PreferenceResponse>), 200)]
        public async Task<IEnumerable<PreferenceResponse>> GetPreferenceAsync() =>
            (await preferenceRepository.GetAllAsync()).Select(mapper.Map<PreferenceResponse>);

    }
}
}
