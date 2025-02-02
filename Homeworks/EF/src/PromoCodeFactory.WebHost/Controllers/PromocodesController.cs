using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Extensions;
using PromoCodeFactory.WebHost.Factories;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromoCodesController : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly Factories.PromoCodeFactory _promoCodeFactory;

        public PromoCodesController(
            IRepository<PromoCode> promoCodeRepository,
            IRepository<Preference> preferenceRepository,
            Factories.PromoCodeFactory promoCodeFactory)
        {
            _promoCodeRepository = promoCodeRepository;
            _preferenceRepository = preferenceRepository;
            _promoCodeFactory = promoCodeFactory;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns>Список промокодов</returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync();
            var response = promoCodes.Select(pc => pc.ToShortResponse()).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <param name="request">Данные для создания промокода</param>
        /// <returns>Результат операции</returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(
            GivePromoCodeRequest request)
        {
            bool result = await _promoCodeFactory.GivePromoCodesToCustomersWithPreferenceAsync(request);

            return Ok(result);
        }
    }
}