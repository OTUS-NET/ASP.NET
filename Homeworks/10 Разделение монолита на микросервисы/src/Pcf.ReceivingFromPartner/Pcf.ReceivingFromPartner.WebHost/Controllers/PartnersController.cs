using Microsoft.AspNetCore.Mvc;
using Pcf.Common.Web.Controllers;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Mapping;
using Pcf.ReceivingFromPartner.WebHost.Models.Partners;

namespace Pcf.ReceivingFromPartner.WebHost.Controllers;

/// <summary>
/// Партнеры
/// </summary>
public class PartnersController(
    IRepository<Partner> partnersRepository,
    IRepository<PartnerPromoCodeLimit> partnerLimitsRepository,
    IAdministrationGateway administrationGateway) : BaseController
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

    /// <summary>
    /// Создать партнера
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PartnerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PartnerResponse>> Create([FromBody] PartnerCreateRequest request, CancellationToken ct)
    {
        // TODO: IAdministrationGateway

        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsActive = request.IsActive,
            ManagerId = request.ManagerId
        };

        await partnersRepository.Add(partner, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { partnerId = partner.Id },
            PartnersMapper.ToPartnerResponse(partner));
    }

    /// <summary>
    /// Обновить партнера
    /// </summary>
    [HttpPut("{partnerId:guid}")]
    [ProducesResponseType(typeof(PartnerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartnerResponse>> Update(
        [FromRoute] Guid partnerId,
        [FromBody] PartnerUpdateRequest request,
        CancellationToken ct)
    {
        var partner = await partnersRepository.GetById(partnerId, withIncludes: true, ct);
        if (partner is null)
            return NotFound();

        // TODO: IAdministrationGateway

        partner.Name = request.Name;
        partner.IsActive = request.IsActive;
        partner.ManagerId = request.ManagerId;

        try
        {
            await partnersRepository.Update(partner, ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

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
