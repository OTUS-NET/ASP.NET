using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        ///  Получить все предпочтения покупателей.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<PreferenceResponse>> GetPreferenceAsync() => 
            (await preferenceRepository.GetAllAsync()).Select(mapper.Map<PreferenceResponse>);
       
    }
}
