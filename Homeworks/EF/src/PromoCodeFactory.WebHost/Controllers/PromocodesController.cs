using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
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
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository;

        private readonly IRepository<Preference> _preferenceRepository;

        private readonly IRepository<Customer> _customerRepository;

        public PromocodesController(IRepository<PromoCode> promoCodeRepository, IRepository<Preference> preferenceRepository, IRepository<Customer> customerRepository)
        {   
            _promoCodeRepository = promoCodeRepository;
            _preferenceRepository = preferenceRepository;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PromoCodeShortResponse>))]
        public async Task<IActionResult> GetPromocodesAsync()
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync();

            var promoCodesModels = promoCodes.Select(pc =>
                    new PromoCodeShortResponse()
                    {
                        Id = pc.Id,
                        Code = pc.Code,
                        ServiceInfo = pc.ServiceInfo,
                        PartnerName = pc.PartnerName,
                        BeginDate = pc.BeginDate.ToString("yyyy-MM-dd"),
                        EndDate = pc.EndDate.ToString("yyyy-MM-dd")
                    });

            return Ok(promoCodesModels);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PromoCodeShortResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {

            var preference = await _preferenceRepository.FirstOrDefaultAsync(p => p.Name == request.Preference);

            if (preference is null)
                return NotFound($"Предпочтение \"{request.Preference}\" не найдено");

            var customer = await _customerRepository.FirstOrDefaultAsync(c => c.Preferences.Any(cp => cp.PreferenceId == preference.Id));

            if (customer is null)
                return NotFound($"Нет пользователей с предпочтением \"{request.Preference}\"");
            
            PromoCode promoCode = new PromoCode
            {
                Id = Guid.NewGuid(),
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,
                PartnerName = request.PartnerName,
                BeginDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(14),
                Preference = preference,
                Customer = customer
            };

            var createdPromoCode = await _promoCodeRepository.AddAsync(promoCode);
            if (createdPromoCode is null)
                return BadRequest("Произошла ошибка при создании промокода.");

            var createdPromoCodeShortResponse = new PromoCodeShortResponse()
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerName = promoCode.PartnerName,
                BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = promoCode.EndDate.ToString("yyyy-MM-dd")
            };
            return Created(string.Empty, createdPromoCodeShortResponse);
        }
    }
}