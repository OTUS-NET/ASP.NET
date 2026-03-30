using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(
    IRepository<PromoCode> promoCodeRepository,
    IRepository<Preference> preferenceRepository,
    IRepository<Employee> employeeRepository,
    IRepository<CustomerPromoCode> customerPromoCodeRepository
    ) : BaseController
{
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var promoCodes = await promoCodeRepository.GetAll(withIncludes: true, ct: ct);
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
        var promoCode = await promoCodeRepository.GetById(id, withIncludes: true, ct: ct);

        if (promoCode is null)
        {
            return NotFound(new ProblemDetails { Title = "Invalid promo code", Detail = $"Promo code with Id {id} not found." });
        }
        return Ok(PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
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
        var preference = await preferenceRepository.GetById(request.PreferenceId, withIncludes: true, ct);
        var manager = await employeeRepository.GetById(request.PartnerManagerId, withIncludes: true, ct);

        if (manager is null || manager.Role.Name is not "PartnerManager")
        {
            return NotFound(new ProblemDetails { Title = "Invalid partner manager id", Detail = $"Partner manager with Id {request.PartnerManagerId} not found." });
        }
        if (preference is null)
        {
            return NotFound(new ProblemDetails { Title = "Invalid preference id", Detail = $"Preference with Id {request.PreferenceId} not found." });
        }
        var promoCode = PromoCodesMapper.ToPromoCode(request, manager, preference);
        await promoCodeRepository.Add(promoCode, ct);
        var targetCustomerIds = preference.Customers.Select(c => c.Id);

        foreach (var targetCustomerId in targetCustomerIds)
        {
            await customerPromoCodeRepository.Add(new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = targetCustomerId,
                PromoCodeId = promoCode.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                AppliedAt = null
            }, ct);
        }
        return Created(promoCode.Id.ToString(), PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
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
        if (await promoCodeRepository.GetById(id, ct: ct) is null)
        {
            return NotFound(new ProblemDetails { Title = "Invalid promo code id", Detail = $"There is no such promo code with id {id}." });
        }
        var customerPromoCodesToApply = await customerPromoCodeRepository.GetWhere(cpc => cpc.PromoCodeId == id && cpc.CustomerId == request.CustomerId, ct: ct);

        if (customerPromoCodesToApply.Count == 0)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid user id", Detail = $"Customer with id {request.CustomerId} does not exist." });
        }
        foreach (var customerPromoCode in customerPromoCodesToApply)
        {
            customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;
            await customerPromoCodeRepository.Update(customerPromoCode, ct);
        }
        return NoContent();
    }
}
