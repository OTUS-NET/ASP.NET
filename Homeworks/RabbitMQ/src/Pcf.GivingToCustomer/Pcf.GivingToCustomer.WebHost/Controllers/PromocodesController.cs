using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.Core.Abstractions.Services.Models;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IPromoCodeIssuingService _promoCodeIssuingService;

        public PromocodesController(IRepository<PromoCode> promoCodesRepository,
            IPromoCodeIssuingService promoCodeIssuingService)
        {
            _promoCodesRepository = promoCodesRepository;
            _promoCodeIssuingService = promoCodeIssuingService;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promocodes = await _promoCodesRepository.GetAllAsync();

            var response = promocodes.Select(x => new PromoCodeShortResponse()
            {
                Id = x.Id,
                Code = x.Code,
                BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                PartnerId = x.PartnerId,
                ServiceInfo = x.ServiceInfo
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            if (!DateTime.TryParse(request.BeginDate, out var beginDate))
                return BadRequest();
            if (!DateTime.TryParse(request.EndDate, out var endDate))
                return BadRequest();

            var ok = await _promoCodeIssuingService.GivePromoCodesToCustomersWithPreferenceAsync(
                new GivePromoCodeToCustomersWithPreferenceCommand
                {
                    PromoCodeId = request.PromoCodeId,
                    PartnerId = request.PartnerId,
                    PromoCode = request.PromoCode,
                    PreferenceId = request.PreferenceId,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    ServiceInfo = request.ServiceInfo
                });

            if (!ok)
                return BadRequest();

            return CreatedAtAction(nameof(GetPromocodesAsync), new { }, null);
        }
    }
}