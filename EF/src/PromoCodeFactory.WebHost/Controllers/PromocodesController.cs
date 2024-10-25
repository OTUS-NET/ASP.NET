using System;
using System.Collections.Generic;
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
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository; 
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync();
            var response = promoCodes.Select(p => new PromoCodeShortResponse
            {
                Id = p.Id,
                Code = p.Code,
                ServiceInfo = p.ServiceInfo,
                BeginDate = p.BeginDate.ToLongDateString(),
                EndDate = p.EndDate.ToShortDateString(),
                PartnerName = p.PartnerName
            }).OrderBy(p => p.Id).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            //TODO: Создать промокод и выдать его клиентам с указанным предпочтением
            throw new NotImplementedException();
        }
    }
}