using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Contracts.PromoCodes;
using PromoCodeFactory.Core.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Контроллер для управления предпочтениями
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PromoCodesController(
    IRepository<Preference> preferenceRepository,
    IRepository<PromoCode> promoCodeRepository,
    IRepository<Customer> customerRepository) : ControllerBase
{
    private readonly IRepository<Preference> _preferenceRepository = preferenceRepository;
    private readonly IRepository<PromoCode> _promoCodeRepository = promoCodeRepository;
    private readonly IRepository<Customer> _customerRepository = customerRepository;

    /// <summary>
    /// Выдать промокоды клиентам с определенным предпочтением
    /// </summary>
    [HttpPost("give-to-customers-with-preference")]
    public async Task<ActionResult<int>> GivePromoCodesToCustomersWithPreferenceAsync(
        [FromBody] GivePromoCodeRequestDto request)
    {
        var preference = await _preferenceRepository.GetByIdAsync(request.PreferenceId);

        if (preference == null)
        {
            return NotFound("Preference not found");
        }

        var customers = await _customerRepository.GetAllAsync();

        if (!customers.Any())
        {
            return NotFound("There are not any customers");
        }

        foreach (var customer in customers)
        {
            await _customerRepository.LoadRelatedDataAsync(customer, c => c.CustomerPreferences);
        }

        var eligibleCustomers = customers
            .Where(c => c.CustomerPreferences != null &&
                        c.CustomerPreferences.Any(cp => cp.PreferenceId == request.PreferenceId))
            .ToList();

        if (!eligibleCustomers.Any())
        {
            return NotFound("There are not eligible customers");
        }

        var promoCodesCreated = 0;

        foreach (var customer in eligibleCustomers)
        {
            var promoCode = new PromoCode
            {
                Code = request.PromoCode + "_" + Guid.NewGuid().ToString("N")[..8],
                ServiceInfo = request.ServiceInfo,
                BeginDate = request.BeginDate,
                EndDate = request.EndDate,
                PartnerName = request.PartnerName,
                PreferenceId = request.PreferenceId,
                CustomerId = customer.Id
            };

            await _promoCodeRepository.CreateAsync(promoCode);

            promoCodesCreated++;
        }

        return Ok(promoCodesCreated);
    }

    /// <summary>
    /// Получить список промокодов
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<PromoCodeResponseDto>>> GetPromoCodesAsync(
        [FromQuery] GetPromoCodeRequestDto request)
    {
        var promoCodes = await _promoCodeRepository.GetAllAsync();

        if (request.PreferenceId.HasValue)
        {
            promoCodes = promoCodes.Where(p => p.PreferenceId == request.PreferenceId.Value);
        }

        if (DateTime.TryParse(request.FromDate, out var parsedFromDate))
        {
            promoCodes = promoCodes.Where(p => p.BeginDate >= parsedFromDate);
        }

        if (DateTime.TryParse(request.ToDate, out var parsedToDate))
        {
            promoCodes = promoCodes.Where(p => p.EndDate <= parsedToDate);
        }

        var promoCodeResponses = promoCodes.Select(p => new PromoCodeResponseDto
        {
            Id = p.Id,
            Code = p.Code,
            ServiceInfo = p.ServiceInfo,
            BeginDate = p.BeginDate,
            EndDate = p.EndDate,
            PartnerName = p.PartnerName,
            PreferenceId = p.PreferenceId
        }).ToList();

        return Ok(promoCodeResponses);
    }
}