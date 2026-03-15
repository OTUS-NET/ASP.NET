using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(
    IRepository<PromoCode> promoCodesRepository,
    IRepository<Customer> customersRepository,
    IRepository<CustomerPromoCode> customerPromoCodesRepository,
    IRepository<Partner> partnersRepository,
    IRepository<Preference> preferencesRepository)
    : BaseController
{
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var promoCodes = await promoCodesRepository.GetAll(withIncludes: true, ct);
        return Ok(promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse));
    }

    /// <summary>
    /// Получить промокод по id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promoCode = await promoCodesRepository.GetById(id, withIncludes: true, ct);
        if (promoCode is null)
            return NotFound();

        return Ok(PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
    }

    /// <summary>
    /// Создать промокод и выдать его клиентам с указанным предпочтением
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PromoCodeShortResponse>> Create(PromoCodeCreateRequest request, CancellationToken ct)
    {
        var partner = await partnersRepository.GetById(request.PartnerId, withIncludes: true, ct: ct);
        if (partner is null)
            return NotFound(new ProblemDetails
            {
                Title = "Partner not found",
                Detail = $"Partner with Id {request.PartnerId} not found."
            });

        var preference = await preferencesRepository.GetById(request.PreferenceId, ct: ct);
        if (preference is null)
            return NotFound(new ProblemDetails
            {
                Title = "Preference not found",
                Detail = $"Preference with Id {request.PreferenceId} not found."
            });

        var customersWithPreference = await customersRepository.GetWhere(
            c => c.Preferences.Any(p => p.Id == request.PreferenceId),
            ct: ct);

        var activeLimit = partner.PartnerLimits
            .FirstOrDefault(l => l.CanceledAt == null && l.EndAt > DateTimeOffset.UtcNow);
        if (activeLimit is null)
            return StatusCode(StatusCodes.Status422UnprocessableEntity, new ProblemDetails
            {
                Title = "No active limit",
                Detail = "Partner has no active promo code limit."
            });

        if (activeLimit.IssuedCount >= activeLimit.Limit)
            return StatusCode(StatusCodes.Status422UnprocessableEntity, new ProblemDetails
            {
                Title = "Limit exceeded",
                Detail = $"Cannot create promo code. Limit would be exceeded (current: {activeLimit.IssuedCount}/{activeLimit.Limit})."
            });

        var promoCodeId = Guid.NewGuid();
        var promoCode = new PromoCode
        {
            Id = promoCodeId,
            Code = request.Code,
            ServiceInfo = request.ServiceInfo,
            Partner = partner,
            BeginDate = request.BeginDate.UtcDateTime,
            EndDate = request.EndDate.UtcDateTime,
            Preference = preference,
            CustomerPromoCodes = customersWithPreference.Select(c => new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = c.Id,
                PromoCodeId = promoCodeId,
                CreatedAt = DateTime.UtcNow,
                AppliedAt = null
            }).ToList()
        };

        foreach (var cpc in promoCode.CustomerPromoCodes)
        {
            cpc.PromoCodeId = promoCode.Id;
        }

        await promoCodesRepository.Add(promoCode, ct);

        activeLimit.IssuedCount += 1;
        await partnersRepository.Update(partner, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = promoCode.Id },
            PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
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
        var promoCode = await promoCodesRepository.GetById(id, ct: ct);
        if (promoCode is null)
            return NotFound();

        var customerPromoCodes = await customerPromoCodesRepository.GetWhere(
            cpc => cpc.PromoCodeId == id && cpc.CustomerId == request.CustomerId,
            ct: ct);
        var customerPromoCode = customerPromoCodes.FirstOrDefault();
        if (customerPromoCode is null)
            return NotFound(new ProblemDetails
            {
                Title = "Customer promo code not found",
                Detail = "The customer was not issued this promo code."
            });

        customerPromoCode.AppliedAt = DateTime.UtcNow;
        await customerPromoCodesRepository.Update(customerPromoCode, ct);

        return NoContent();
    }
}
