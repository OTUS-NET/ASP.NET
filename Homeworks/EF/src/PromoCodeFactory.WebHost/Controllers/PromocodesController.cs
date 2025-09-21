using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
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
    public class PromocodesController: ControllerBase
    {
        private readonly IRepository<PromoCode> _repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public PromocodesController(IRepository<PromoCode> repository, ICustomerRepository customerRepository,IRepository<Employee> employeeRepository)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
        }
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            //TODO: Получить все промокоды 
            var promoCodes = await _repository.GetAllAsync();
            var response = promoCodes.Select(x => 
            new PromoCodeShortResponse() 
            {
                Id  = x.Id,
                Code = x.Code,
                ServiceInfo  = x.ServiceInfo,
                BeginDate  = x.BeginDate.ToLongDateString(),
                EndDate  = x.EndDate.ToLongDateString(),
                PartnerName  = x.PartnerName
            }).ToList();
            return response;
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            //TODO: Создать промокод и выдать его клиентам с указанным предпочтением
            if (string.IsNullOrEmpty(request.ServiceInfo) || string.IsNullOrEmpty(request.PartnerName) || string.IsNullOrEmpty(request.PromoCode))
                return BadRequest("Не заполнены обязательные поля");

            var existingPromoCode = (await _repository.GetAllAsync())
                .FirstOrDefault(e => e.Code.Equals(request.PromoCode, StringComparison.OrdinalIgnoreCase));

            if (existingPromoCode != null)
                return Conflict("Промокод уже существует");

            var promoCode = new PromoCode()
            {
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                PartnerName = request.PartnerName,
            };

            var customres = await _customerRepository.GetCustomersWithAllProperties();
            var requiredCustomers = customres.Where(x => x.CustomerPreferences.Any(x => x.Preference.Name == request.Preference)).ToList();

            if (requiredCustomers is null || !requiredCustomers.Any())
                await _repository.AddAsync(promoCode);
            else
            {
                requiredCustomers.ForEach(c =>
                {
                    promoCode.CustomerId = c.Id;
                    _repository.AddAsync(promoCode);
                });
            }
            return Ok(promoCode);
        }
    }
}