using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(IRepository<PromoCode> promoCodeRepository,
    IRepository<Employee> employeeRepository,
    IRepository<Preference> preferenceRepository,
    IRepository<Customer> customerRepository,
    IRepository<CustomerPromoCode> customerPromoCodeRepository) : BaseController
{
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var promoCodes = await promoCodeRepository.GetAll(true, ct);

        var promoCedesModel = promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse);

        return Ok(promoCedesModel);
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

        if (promoCode == null)
            return NotFound();

        var promoCodeModel = PromoCodesMapper.ToPromoCodeShortResponse(promoCode);

        return Ok(promoCodeModel);
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
        if(partnerManager == null)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid partner manager",
                Detail = $"Partner manager with Id {request.PartnerManagerId} not found."
            });

        var preference = await preferenceRepository.GetById(request.PreferenceId, ct: ct);
        if (preference == null)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid preference",
                Detail = $"Preference with id {request.PreferenceId} not found."
            });

        var customersWithPreference = await customerRepository.GetWhere(x => x.Preferences.Contains(preference), ct: ct);
        if(!customersWithPreference.Any())
            return NotFound(new ProblemDetails
            {
                Detail = $"Customers with preference: {preference.Name} not found."
            });

        var customerPromoCodes = customersWithPreference.Select(x => new CustomerPromoCode
        {
            Id = Guid.NewGuid(),
            CustomerId = x.Id,
            CreatedAt = DateTime.UtcNow
        });

        var promoCode = PromoCodesMapper.ToPromoCode(request, partnerManager, preference, customerPromoCodes);

        await promoCodeRepository.Add(promoCode, ct);

        var promoCodeModel = PromoCodesMapper.ToPromoCodeShortResponse(promoCode);

        return CreatedAtAction(nameof(GetById), new { id = promoCode.Id }, promoCodeModel);
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
        var customerPromoCodes = await customerPromoCodeRepository.GetWhere(x => x.CustomerId == request.CustomerId
            && x.PromoCodeId == id && x.AppliedAt == null, ct: ct);

        if (!customerPromoCodes.Any())
            return NotFound(new ProblemDetails
            {
                Detail = $"No active promo code with id {id} for the customer with id {request.CustomerId}"
            });

        var customerPromoCode = customerPromoCodes.First();

        customerPromoCode.AppliedAt = DateTime.UtcNow;
        await customerPromoCodeRepository.Update(customerPromoCode, ct);

        return NoContent();
    }
}
