using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(
    IRepository<PromoCode> promoCodeRepository,
    IRepository<Employee> employeeRepository,
    IRepository<Preference> preferenceRepository,
    IRepository<Customer> customerRepository,
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
        var promoCodes = await promoCodeRepository.GetAll(true, ct);

        var promoCodesModels = promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse).ToList();

        return Ok(promoCodesModels);
    }

    /// <summary>
    /// Получить промокод по id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promoCode = await promoCodeRepository.GetById(id, true, ct);

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
    public async Task<ActionResult<PromoCodeShortResponse>> Create(PromoCodeCreateRequest request, CancellationToken ct)
    {
        var partnerManager = await employeeRepository.GetById(request.PartnerManagerId, ct: ct);
        if (partnerManager is null)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid partner manager",
                Detail = $"Employee with Id {request.PartnerManagerId} not found."
            });

        var preference = await preferenceRepository.GetById(request.PreferenceId, ct: ct);
        if (preference is null)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid preference",
                Detail = $"Preference with Id {request.PreferenceId} not found."
            });
        //создание промокода
        var promoCode = PromoCodesMapper.ToPromoCode(request, partnerManager, preference);
        //производится выдача клиентам с указанным предпочтением
        var customers = await customerRepository.GetWhere(
            c => c.Preferences.Any(p => p.Id == request.PreferenceId), ct: ct);

        promoCode.CustomerPromoCodes = customers
            .Select(c => new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = c.Id,
                PromoCodeId = promoCode.Id,
                CreatedAt = DateTimeOffset.UtcNow
            })
            .ToList();

        await promoCodeRepository.Add(promoCode, ct);

        return CreatedAtAction(nameof(GetById), new { id = promoCode.Id }, PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
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
        var promoCode = await promoCodeRepository.GetById(id, ct: ct);
        if (promoCode is null)
            return NotFound();

        var customerPromoCodes = await customerPromoCodeRepository.GetWhere(
            cpc => cpc.PromoCodeId == id && cpc.CustomerId == request.CustomerId, ct: ct);

        var customerPromoCode = customerPromoCodes.FirstOrDefault();
        if (customerPromoCode is null)
            return BadRequest(new ProblemDetails
            {
                Title = "Promo code not issued",
                Detail = $"Promo code with Id {id} was not issued to customer with Id {request.CustomerId}."
            });

        if (customerPromoCode.AppliedAt is not null)
            return BadRequest(new ProblemDetails
            {
                Title = "Promo code already applied",
                Detail = $"Promo code with Id {id} has already been applied by customer with Id {request.CustomerId}."
            });

        customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;
        await customerPromoCodeRepository.Update(customerPromoCode, ct);

        return NoContent();
    }
}
