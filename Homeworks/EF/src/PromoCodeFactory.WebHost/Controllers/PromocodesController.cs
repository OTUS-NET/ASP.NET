using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.BusinessLogic;
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
        private IRepository<PromoCode> _promocodeRepository;
        private PromoCodeFactoryBl _promoCodeFactory;
        public PromocodesController(
            IRepository<PromoCode> promocodeRepository,
            PromoCodeFactoryBl promoCodeFactory)
        {
            _promocodeRepository = promocodeRepository;
            _promoCodeFactory = promoCodeFactory;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var dbResult = await _promocodeRepository.GetAllAsync();
            var result = new List<PromoCodeShortResponse>(
                dbResult.Select(x => new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    Code = x.Code,
                    ServiceInfo = x.ServiceInfo,
                    BeginDate = x.BeginDate.ToString(),
                    EndDate = x.EndDate.ToString(),
                    PartnerName = x.PartnerName
                }));
            return Ok(result);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением ( два поля в запросе изменены на гуиды)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType<PreferenceResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            bool result = await _promoCodeFactory.GivePromoCodesToCustomersWithPreferenceAsync(request);
            return result 
                ? Created("api/v1/Promocodes", result) 
                : BadRequest("Ошибка при добавлении промокода, см. логи сервера, не стал уже пробрасывать сюда мессадж");
        }
    }
}