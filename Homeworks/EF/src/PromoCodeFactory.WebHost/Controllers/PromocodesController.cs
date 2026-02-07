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
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IRepository<CustomerPreference> _customerPreferenceRepository;

        public PromocodesController(
            IRepository<PromoCode> promoCodeRepository,
            IRepository<Preference> preferenceRepository,
            IRepository<CustomerPreference> customerPreferenceRepository)
        {
            _promoCodeRepository = promoCodeRepository;
            _preferenceRepository = preferenceRepository;
            _customerPreferenceRepository = customerPreferenceRepository;
        }

        /// <summary>
        /// Получить все промокоды (с фильтрацией по датам)
        /// </summary>
        /// <param name="beginDate">Дата начала действия промокода (yyyy-MM-dd)</param>
        /// <param name="endDate">Дата окончания действия промокода (yyyy-MM-dd)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(
            [FromQuery] string beginDate = null,
            [FromQuery] string endDate = null)
        {
            DateTime? begin = null;
            DateTime? end = null;

            if (!string.IsNullOrWhiteSpace(beginDate) &&
                DateTime.TryParseExact(beginDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var beginParsed))
            {
                begin = beginParsed.Date;
            }

            if (!string.IsNullOrWhiteSpace(endDate) &&
                DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endParsed))
            {
                end = endParsed.Date;
            }

            var promoCodes = await _promoCodeRepository.GetAllAsync();

            var filtered = promoCodes.AsEnumerable();
            if (begin.HasValue)
                filtered = filtered.Where(p => p.BeginDate.Date >= begin.Value);
            if (end.HasValue)
                filtered = filtered.Where(p => p.EndDate.Date <= end.Value);

            var response = filtered
                .Select(p => new PromoCodeShortResponse
                {
                    Id = p.Id,
                    Code = p.Code,
                    ServiceInfo = p.ServiceInfo,
                    BeginDate = p.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = p.EndDate.ToString("yyyy-MM-dd"),
                    PartnerName = p.PartnerName
                })
                .ToList();

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            if (request == null)
                return BadRequest("Request is null");

            if (string.IsNullOrWhiteSpace(request.Preference))
                return BadRequest("Preference is required");

            if (string.IsNullOrWhiteSpace(request.PromoCode))
                return BadRequest("PromoCode is required");

            var preference = (await _preferenceRepository.GetAllAsync())
                .FirstOrDefault(p => p.Name == request.Preference);

            if (preference == null)
                return NotFound($"Preference '{request.Preference}' not found");

            var customersWithPreference = (await _customerPreferenceRepository.GetAllAsync())
                .Where(cp => cp.PreferenceId == preference.Id)
                .Select(cp => cp.CustomerId)
                .Distinct()
                .ToList();

            var begin = DateTime.UtcNow.Date;
            var end = begin.AddMonths(1);

            foreach (var customerId in customersWithPreference)
            {
                var promoCode = new PromoCode
                {
                    Id = Guid.NewGuid(),
                    Code = request.PromoCode,
                    ServiceInfo = request.ServiceInfo,
                    PartnerName = request.PartnerName,
                    BeginDate = begin,
                    EndDate = end,
                    CustomerId = customerId,
                    PreferenceId = preference.Id
                };

                await _promoCodeRepository.AddAsync(promoCode);
            }

            return Ok(customersWithPreference.Count);
        }
    }
}