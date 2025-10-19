using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController(
        IRepository<PromoCode> promocodeRepository,
        IPreferenceRepository preferenceRepository,
        IRepository<Employee> employeeRepository,
        ICustomerRepository customerRepository
        )
        : ControllerBase
    {
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var result = await promocodeRepository.GetAllAsync();

            var promoCodeShortResponse = result.Select(x => new PromoCodeShortResponse
            {
                Id = x.Id,
                Code = x.Code,
                PartnerName = x.PartnerName,
                BeginDate = x.BeginDate.ToString(),
                EndDate = x.EndDate.ToString(),
                ServiceInfo = x.ServiceInfo
            });

            return Ok(promoCodeShortResponse);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var preference = await preferenceRepository.GetByName(request.Preference);

            if (preference == null)
                return BadRequest(request.Preference);

            var employee = await employeeRepository.GetByIdAsync(request.PartnerManagerId);

            if (employee == null)
                return BadRequest(request);

            var customers = await customerRepository.GetAllByPreference(preference.Id);

            foreach (var customer in customers)
            {
                var promoCode = new PromoCode
                {
                    Id = Guid.NewGuid(),
                    Code = request.PromoCode,
                    PartnerName = request.PartnerName,
                    PartnerManagerId = request.PartnerManagerId,
                    BeginDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    ServiceInfo = request.ServiceInfo,
                    CustomerId = customer.Id,
                    PreferenceId = preference.Id
                };

                try
                {
                    await promocodeRepository.AddAsync(promoCode);
                }
                catch (DbUpdateException ex)
                {
                    // Детальная диагностика
                    Console.WriteLine($"Failed to create PromoCode:");
                    Console.WriteLine($"PartnerManagerId: {request.PartnerManagerId}");
                    Console.WriteLine($"CustomerId: {customer.Id}");
                    Console.WriteLine($"Error: {ex.InnerException?.Message}");
                    throw;
                }
            }

            return Ok();
        }
    }
}