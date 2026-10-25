using Microsoft.AspNetCore.Mvc;
using Pcf.Common.Web.Controllers;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Mapping;
using Pcf.ReceivingFromPartner.WebHost.Models.PromoCodes;

namespace Pcf.ReceivingFromPartner.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(
    IRepository<PromoCode> promoCodesRepository,
    IRepository<Partner> partnersRepository,
    IRepository<Preference> preferencesRepository,
    IGivingToCustomerGateway givingToCustomerGateway)
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
            Preference = preference
        };

        await promoCodesRepository.Add(promoCode, ct);

        activeLimit.IssuedCount += 1;
        await partnersRepository.Update(partner, ct);

        // TODO: IGivingToCustomerGateway

        return CreatedAtAction(
            nameof(GetById),
            new { id = promoCode.Id },
            PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
    }
}
