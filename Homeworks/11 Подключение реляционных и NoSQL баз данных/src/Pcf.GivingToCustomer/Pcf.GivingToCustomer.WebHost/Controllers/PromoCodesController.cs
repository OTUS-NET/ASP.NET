using Microsoft.AspNetCore.Mvc;
using Pcf.Common.Web.Controllers;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mapping;
using Pcf.GivingToCustomer.WebHost.Models.PromoCodes;

namespace Pcf.GivingToCustomer.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(
    IRepository<PromoCode> promoCodesRepository,
    IRepository<Customer> customersRepository,
    IRepository<CustomerPromoCode> customerPromoCodesRepository,
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> Create(CreatePromoCodeRequest request, CancellationToken ct)
    {
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

        var promoCode = new PromoCode
        {
            Id = request.Id,
            Code = request.Code,
            ServiceInfo = request.ServiceInfo,
            BeginDate = request.BeginDate,
            EndDate = request.EndDate,
            PartnerId = request.PartnerId,
            Preference = preference,
            CustomerPromoCodes = customersWithPreference.Select(c => new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = c.Id,
                PromoCodeId = request.Id,
                CreatedAt = DateTimeOffset.UtcNow
            }).ToList()
        };

        await promoCodesRepository.Add(promoCode, ct);

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

        customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;
        await customerPromoCodesRepository.Update(customerPromoCode, ct);

        return NoContent();
    }
}
