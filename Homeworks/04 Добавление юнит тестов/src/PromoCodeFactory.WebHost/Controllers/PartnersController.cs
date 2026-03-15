using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Partners;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Партнеры
/// </summary>
public class PartnersController(
    IRepository<Partner> partnersRepository,
    IRepository<PartnerPromoCodeLimit> partnerLimitsRepository) : BaseController
{
    /// <summary>
    /// Получить данные всех партнеров
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PartnerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PartnerResponse>>> Get(CancellationToken ct)
    {
        var partners = await partnersRepository.GetAll(withIncludes: true, ct: ct);
        var response = partners.Select(PartnersMapper.ToPartnerResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Получить данные партнера по Id
    /// </summary>
    [HttpGet("{partnerId:guid}")]
    [ProducesResponseType(typeof(PartnerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartnerResponse>> GetById([FromRoute] Guid partnerId, CancellationToken ct)
    {
        var partner = await partnersRepository.GetById(partnerId, withIncludes: true, ct);
        if (partner is null)
            return NotFound();

        return Ok(PartnersMapper.ToPartnerResponse(partner));
    }

    [HttpGet("{partnerId:guid}/limits/{limitId:guid}")]
    [ProducesResponseType(typeof(PartnerPromoCodeLimitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartnerPromoCodeLimitResponse>> GetLimit(
        [FromRoute] Guid partnerId,
        [FromRoute] Guid limitId,
        CancellationToken ct)
    {
        var partner = await partnersRepository.GetById(partnerId, withIncludes: true, ct);
        if (partner is null)
            return NotFound();

        var limit = partner.PartnerLimits.FirstOrDefault(l => l.Id == limitId);
        if (limit is null)
            return NotFound();

        return Ok(PartnersMapper.ToPartnerPromoCodeLimitResponse(limit));
    }

    [HttpPost("{partnerId:guid}/limits")]
    [ProducesResponseType(typeof(PartnerPromoCodeLimitResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PartnerPromoCodeLimitResponse>> CreateLimit(
        [FromRoute] Guid partnerId,
        [FromBody] PartnerPromoCodeLimitCreateRequest request,
        CancellationToken ct)
    {
        var partner = await partnersRepository.GetById(partnerId, withIncludes: true, ct);
        if (partner is null)
            return NotFound(new ProblemDetails
            {
                Title = "Partner not found",
                Detail = $"Partner with Id {partnerId} not found."
            });

        if (!partner.IsActive)
            return UnprocessableEntity(new ProblemDetails
            {
                Title = "Partner blocked",
                Detail = "Cannot create limit for a blocked partner."
            });

        var now = DateTimeOffset.UtcNow;

        // Отключить предыдущие активные лимиты
        var activeLimits = partner.PartnerLimits.Where(l => l.CanceledAt == null).ToList();

        foreach (var activeLimit in activeLimits)
        {
            activeLimit.CanceledAt = now;
        }

        if (activeLimits.Count > 0)
        {
            try
            {
                await partnersRepository.Update(partner, ct);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        var newLimit = new PartnerPromoCodeLimit
        {
            Id = Guid.NewGuid(),
            Partner = partner,
            CreatedAt = now,
            EndAt = request.EndAt,
            Limit = request.Limit,
            IssuedCount = 0
        };

        await partnerLimitsRepository.Add(newLimit, ct);

        return CreatedAtAction(
            nameof(GetLimit),
            new { partnerId, limitId = newLimit.Id },
            PartnersMapper.ToPartnerPromoCodeLimitResponse(newLimit));
    }

    [HttpPost("{partnerId:guid}/limits/{limitId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CancelLimit(
        [FromRoute] Guid partnerId,
        [FromRoute] Guid limitId,
        CancellationToken ct)
    {
        var partner = await partnersRepository.GetById(partnerId, withIncludes: true, ct);
        if (partner is null)
            return NotFound(new ProblemDetails
            {
                Title = "Partner not found",
                Detail = $"Partner with Id {partnerId} not found."
            });

        if (!partner.IsActive)
            return UnprocessableEntity(new ProblemDetails
            {
                Title = "Partner blocked",
                Detail = "Cannot cancel limit for a blocked partner."
            });

        var limit = partner.PartnerLimits.FirstOrDefault(l => l.Id == limitId);
        if (limit is null)
            return NotFound(new ProblemDetails
            {
                Title = "Limit not found",
                Detail = $"Limit with Id {limitId} not found."
            });

        if (limit.CanceledAt != null)
            return UnprocessableEntity(new ProblemDetails
            {
                Title = "Limit already canceled",
                Detail = "This limit has already been canceled."
            });

        limit.CanceledAt = DateTimeOffset.UtcNow;

        try
        {
            await partnersRepository.Update(partner, ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}
