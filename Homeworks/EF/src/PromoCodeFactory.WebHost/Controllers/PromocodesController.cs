using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
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
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public PromocodesController(
            IRepository<PromoCode> promoCodeRepository,
            IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository,
            IRepository<Employee> employeeRepository)
        {
            _promoCodeRepository = promoCodeRepository;
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить все промокоды
        /// beginDate и endDate передаются в формате yyyy-MM-dd строкой.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(
            [FromQuery] string? beginDate,
            [FromQuery] string? endDate)
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(beginDate))
            {
                if (!DateTime.TryParseExact(
                        beginDate,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var parsedBeginDate))
                {
                    return BadRequest("beginDate must be in format yyyy-MM-dd");
                }

                promoCodes = promoCodes.Where(x => x.BeginDate.Date >= parsedBeginDate.Date);
            }

            if (!string.IsNullOrWhiteSpace(endDate))
            {
                if (!DateTime.TryParseExact(
                        endDate,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var parsedEndDate))
                {
                    return BadRequest("endDate must be in format yyyy-MM-dd");
                }

                promoCodes = promoCodes.Where(x => x.EndDate.Date <= parsedEndDate.Date);
            }

            var response = promoCodes.Select(x => new PromoCodeShortResponse
            {
                Id = x.Id,
                Code = x.Code,
                ServiceInfo = x.ServiceInfo,
                BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                PartnerName = x.PartnerName
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync([FromBody] GivePromoCodeRequest request)
        {
            var preference = (await _preferenceRepository.GetAllAsync())
                .FirstOrDefault(x => x.Name == request.Preference);

            if (preference == null)
            {
                return BadRequest("Preference not found");
            }

            var partnerManager = (await _employeeRepository.GetAllAsync())
                .FirstOrDefault(x => x.Role.Name == "PartnerManager");

            if (partnerManager == null)
            {
                return BadRequest("Partner manager not found");
            }

            var customers = (await _customerRepository.GetAllAsync())
                .Where(x => x.CustomerPreferences.Any(cp => cp.PreferenceId == preference.Id))
                .ToList();

            foreach (var customer in customers)
            {
                var promoCode = new PromoCode
                {
                    Id = Guid.NewGuid(),
                    Code = request.PromoCode,
                    ServiceInfo = request.ServiceInfo,
                    BeginDate = DateTime.UtcNow.Date,
                    EndDate = DateTime.UtcNow.Date.AddMonths(1),
                    PartnerName = request.PartnerName,
                    PartnerManagerId = partnerManager.Id,
                    PreferenceId = preference.Id,
                    CustomerId = customer.Id
                };

                await _promoCodeRepository.CreateAsync(promoCode);
            }

            return NoContent();
        }
    }
}