using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class PromocodesController(
        ILogger<PromocodesController> logger,
        IRepository<PromoCode> codesRepository,
        IRepository<Preference> preferencesRepository,
        IRepository<Customer> customersRepository,
        IRepository<Employee> employeeRepository
    ) : ControllerBase
    {
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            try
            {
                var codes = await codesRepository.GetAllAsync();

                return Ok(codes.Select(x => new PromoCodeShortResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    BeginDate = x.BeginDate.ToString(CultureInfo.InvariantCulture),
                    EndDate = x.EndDate.ToString(CultureInfo.InvariantCulture),
                    PartnerName = x.PartnerName,
                    ServiceInfo = x.ServiceInfo
                }).ToList());
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error get promocodes list: [{msg}]", e.Message);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            try
            {
                var preference = (await preferencesRepository.GetAllAsync())
                    .FirstOrDefault(x => x.Name == request.Preference);

                if (preference == null)
                    return BadRequest("Preference not found");

                var employees = await employeeRepository.GetAllAsync();
                var code = new PromoCode
                {
                    Id = Guid.NewGuid(),
                    PartnerName = request.PartnerName,
                    ServiceInfo = request.ServiceInfo,
                    Code = request.PromoCode,
                    Preference = preference,

                    BeginDate = DateTime.UtcNow,
                    Customers = preference.Customers.ToList(),
                    PartnerManager = employees.Single(x => x.FullName == request.PartnerName)
                };

                await codesRepository.AddAsync(code);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error apply promocodes: [{msg}]", e.Message);
                return StatusCode(500, e.Message);
            }
        }
    }
}