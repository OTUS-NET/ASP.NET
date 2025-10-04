using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IRepository<Customer> _customersRepository;
        private readonly IPreferencesDirectoryGateway _preferencesDirectoryGateway;

        public PromocodesController(IRepository<PromoCode> promoCodesRepository, 
            IRepository<Customer> customersRepository,
            IPreferencesDirectoryGateway preferencesDirectoryGateway)
        {
            _promoCodesRepository = promoCodesRepository;
            _customersRepository = customersRepository;
            _preferencesDirectoryGateway = preferencesDirectoryGateway;
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
            //Получаем предпочтение по имени
            var preference = await _preferencesDirectoryGateway.GetPreferenceByIdAsync(request.PreferenceId);
            if (preference == null)
            {
                return BadRequest();
            }

            //  Получаем клиентов с этим предпочтением:
            var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.PreferenceId == preference.Id));

            PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preference.Id, customers);

            await _promoCodesRepository.AddAsync(promoCode);

            return CreatedAtAction(nameof(GetPromocodesAsync), new { }, null);
        }
    }
}