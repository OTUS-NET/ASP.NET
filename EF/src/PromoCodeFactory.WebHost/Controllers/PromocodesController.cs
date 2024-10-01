using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Utils;

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
        private readonly IRepository<PromoCode> _repo;

        public PromocodesController(IRepository<PromoCode> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(CancellationToken token)
        {
            var promoCodes = await _repo.GetAllAsync(token);
            return Ok(promoCodes.ToShortResponseList());
        }

        /// <summary>
        /// Создать промокод и выдать его определенному клиенту.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(Guid id, GivePromoCodeRequest request, CancellationToken token)
        {
            var promo = new PromoCode
            {
                Id = Guid.NewGuid(),
                Code = request.PromoCode,
                BeginDate = DateTime.UtcNow,
                ServiceInfo = request.ServiceInfo,
                PartnerName = request.PartnerName,
                Preference = new Preference { Name = request.Preference },
                CustomerId = id,
            };

            await _repo.CreateAsync(promo, token);
            return Ok();
        }
    }
}