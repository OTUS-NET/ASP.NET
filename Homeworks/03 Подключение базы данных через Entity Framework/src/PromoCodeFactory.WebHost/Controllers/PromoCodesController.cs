using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Preferences;
using PromoCodeFactory.WebHost.Models.PromoCodes;
using System.Reflection.Metadata.Ecma335;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(IRepository<PromoCode> promoCodeRepository, IRepository<Customer> customerRepository,
    IRepository<Preference> preferenceRepository,
    IRepository<Employee> employeeRepository) : BaseController
{
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var preferences = await promoCodeRepository.GetAll(withIncludes: true, ct);

        var response = preferences.Select(p => new PromoCodeShortResponse(
            p.Id,
            p.Code,
            p.ServiceInfo,
            p.PartnerName,
            p.EndDate,
            p.BeginDate,
            p.PartnerManager.Id,
            p.Preference.Id)).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Получить промокод по id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promocode = await promoCodeRepository.GetById(id, withIncludes: true, ct);

        if (promocode == null)
            return NotFound();

        var response =  new PromoCodeShortResponse(
            promocode.Id,
            promocode.Code,
            promocode.ServiceInfo,
            promocode.PartnerName,
            promocode.EndDate,
            promocode.BeginDate,
            promocode.PartnerManager.Id,
            promocode.Preference.Id);

        return Ok(response);
    }

    /// <summary>
    /// Создать промокод и выдать его клиентам с указанным предпочтением
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> Create(PromoCodeCreateRequest request, CancellationToken ct)
    {
        var preference = await preferenceRepository.GetById(request.PreferenceId, ct: ct);

        var partnerManager = await employeeRepository.GetById(request.PartnerManagerId, ct: ct);

        if (preference == null || partnerManager == null)
            return NotFound();

        var customers = await customerRepository.GetWhere(
            c => c.Preferences.Any(p => p.Id == request.PreferenceId),
            withIncludes: true, ct: ct
        );

        var promoCode = new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            ServiceInfo = request.ServiceInfo,
            PartnerName = request.PartnerName,
            BeginDate = DateTimeOffset.UtcNow,
            EndDate = DateTimeOffset.MaxValue,
            Preference = preference,
            PartnerManager = partnerManager,
            CustomerPromoCodes = []
        };

        foreach (var customer in customers)
        {
            promoCode.CustomerPromoCodes.Add(new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                PromoCodeId = promoCode.Id,
                CreatedAt = promoCode.BeginDate
            });
        }

        await promoCodeRepository.Add(promoCode, ct);

        return Created();
    }

    /// <summary>
    /// Применить промокод (отметить, что клиент использовал промокод)
    /// </summary>
    [HttpPost("{id:guid}/apply")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Apply(
        [FromRoute] Guid id,
        [FromBody] PromoCodeApplyRequest request,
        CancellationToken ct)
    {
        var customer = await customerRepository.GetById(request.CustomerId, withIncludes: true, ct);

        if (customer == null)
            return NotFound(); 

        var customerPromo = customer.CustomerPromoCodes.FirstOrDefault(c => c.PromoCodeId == id);

        if (customerPromo == null)
            return NotFound();

        if (customerPromo.AppliedAt.HasValue)
            return BadRequest();

        customerPromo.AppliedAt = DateTimeOffset.UtcNow;

        await customerRepository.Update(customer, ct);

        return NoContent();
    }
}
