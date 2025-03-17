using System;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using PromoCodeFactory.WebHost.Models.Request;
using PromoCodeFactory.WebHost.Models.Response;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromoCodeController(IRepository<PromoCode, Guid> promoCodeRepository,
        IRepository<Employee, Guid> employeeRepository,
        IRepository<Preference, Guid> preferenceRepository,
        ICustomerRepository costomerRepository,
        IMapper mapper) : ControllerBase
    {
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), 200)]
        public async Task<IEnumerable<PromoCodeShortResponse>> GetPromocodesAsync() =>
            // Получаем все промокоды
            (await promoCodeRepository.GetAllAsync()).Select(mapper.Map<PromoCodeShortResponse>);

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync([FromBody] GivePromoCodeRequest request)
        {
            var costomer = mapper.Map<CustomerResponse>(await costomerRepository.GetByIdAsync(request.CustomerId));
            var preference = await preferenceRepository.GetByIdAsync(request.PreferenceId);
            if (costomer == null) return NotFound("The customer with this Id was not found");
            if ((await employeeRepository.GetByIdAsync(request.EmployeeId)) == null) return NotFound("The employee with this Id was not found");
            if (preference == null) return NotFound("The preference with this Id was not found");
            //Проверка наличия продпочтений.
            if (costomer.Preferences.Where(p => p.Name == preference.Name).FirstOrDefault() == null)
                return BadRequest("The customer does not have such preferences");

            var promoCode = mapper.Map<PromoCode>(request);
            promoCode.BeginDate = DateTime.Now.AddDays(request.BeforeStarts);
            promoCode.EndDate = promoCode.BeginDate.AddDays(request.HowLongDay);
            await promoCodeRepository.CreateAsync(promoCode);
            return NoContent();
        }
    }
}
