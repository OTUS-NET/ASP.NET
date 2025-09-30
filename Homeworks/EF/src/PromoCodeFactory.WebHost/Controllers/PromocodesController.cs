using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController(
        IRepository<PromoCode> codesRepository,
        IRepository<Preference> preferencesRepository
        ) : ControllerBase
    {
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var codes = await codesRepository.GetAllAsync();

            return Ok(codes.Select(x => new PromoCodeShortResponse
            {
                Id = x.Id,
                Code = x.Code,
                BeginDate = x.BeginDate.ToString(CultureInfo.InvariantCulture),
                EndDate = x.EndDate.ToString(CultureInfo.InvariantCulture),
                PartnerName = x.PartnerName,
                ServiceInfo = x.ServiceInfo
            }).ToList());
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var preference = (await preferencesRepository.GetAllAsync())
                .FirstOrDefault(x => x.Name == request.Preference);
            
            if (preference == null)
                return BadRequest("Preference not found");
                
            var code = new PromoCode
            {
                Id = Guid.NewGuid(),
                PartnerName = request.PartnerName,
                ServiceInfo = request.ServiceInfo,
                Code = request.PromoCode,
                Preference = preference,
                
                //TODO: fix this
                BeginDate = DateTime.UtcNow, 
                Customer = null,
                PartnerManager = null
            };

            code = await codesRepository.AddAsync(code);
            
            foreach (var customer in preference.Customers)
            {
                customer.PromoCodes.Add(code);
            }
            
            await preferencesRepository.UpdateAsync(preference);
            return Ok(code);
        }
    }
}