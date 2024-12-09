using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories.Abstractions;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController : ControllerBase
    {
        private readonly IRepository<PromoCode> _promocodeRepository;

        private readonly IRepository<Customer> _customerRepository;

        private readonly IRepository<Employee> _employeeRepository;

        private readonly IPreferenceRepositoriy _preferenceRepository;

        public PromocodesController(IRepository<PromoCode> promocodeRepository, 
                                    IRepository<Customer> customerRepository, 
                                    IRepository<Employee> employeeRepository,
                                    IPreferenceRepositoriy preferenceRepository)
        {
            _promocodeRepository = promocodeRepository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _preferenceRepository = preferenceRepository;
        }
        
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(CancellationToken cancellationToken)
        {
            var promocodes = await _promocodeRepository.GetAllAsync(cancellationToken);

            var shortPromocodes = promocodes.Select(x => new PromoCodeShortResponse
            {
                BeginDate = x.BeginDate.ToShortDateString(),
                EndDate = x.EndDate.ToShortDateString(),
                Code = x.Code,
                ServiceInfo = x.ServiceInfo,
                Id = x.Id,
                PartnerName = x.PartnerName
            });

            return Ok(shortPromocodes);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request, CancellationToken cancellationToken)
        {
            var preference = await _preferenceRepository.GetByName(request.Preference);

            if (preference==null)
                return BadRequest(request.Preference);

            var partnerManagers = await _employeeRepository.GetAllAsync(cancellationToken);

            var partnerManager = partnerManagers.FirstOrDefault(x => x.FullName == request.PartnerName);
            if (partnerManager == null)
                return BadRequest(request);

            var customers = await _customerRepository.GetAllAsync(cancellationToken);

            var customersWithPreference = customers
                .Where(x => x.CustomerPreferences != null && x.CustomerPreferences.Any(c => c.PreferenceId == preference.Id))
                .ToList();

            foreach (var customer in customersWithPreference)
            {
                PromoCode promoCode = new PromoCode
                {
                    BeginDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(7),
                    Code = request.PromoCode,
                    CustomerId = customer.Id,
                    PartnerName = request.PartnerName,
                    ServiceInfo = request.ServiceInfo,
                    Preference = preference,
                    PartnerManager = partnerManager,
                };

                await _promocodeRepository.CreateAsync(promoCode, cancellationToken);
            }

            return Ok();
        }
    }
}