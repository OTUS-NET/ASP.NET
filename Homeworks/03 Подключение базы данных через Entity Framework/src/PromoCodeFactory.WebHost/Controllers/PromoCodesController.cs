using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
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
    IRepository<Customer> customerRepository
    ) : BaseController
{
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var promoCodes = await promoCodeRepository.GetAll(ct: ct);

        var promoCodesModel = promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse).ToList();

        return Ok(promoCodesModel);
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
        var preference = await preferenceRepository.GetById(request.PreferenceId, ct: ct);
        if (preference is null)
            return BadRequest(new ProblemDetails { Title = "Invalid preference", Detail = $"Preference with Id {request.PreferenceId} not found." });

        var partnerManager = await employeeRepository.GetById(request.PartnerManagerId, ct: ct);
        if (partnerManager is null)
            return BadRequest(new ProblemDetails { Title = "Invalid partner manager", Detail = $"Employee with Id {request.PartnerManagerId} not found." });

        var promoCode = PromoCodesMapper.ToPromoCode(request, preference, partnerManager);
        await promoCodeRepository.Add(promoCode, ct);

        var customersWithPreference = await customerRepository.GetWhere(c => c.Preferences.Any(p => p.Id == request.PreferenceId), true, ct);

        foreach (var customer in customersWithPreference)
        {
            var customerPromoCode = new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                PromoCodeId = promoCode.Id,
                CreatedAt = DateTimeOffset.UtcNow
            };
            customer.CustomerPromoCodes.Add(customerPromoCode);
            await customerRepository.Update(customer, ct);
        }

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
        var promoCode = await promoCodeRepository.GetById(id, true, ct);
        if (promoCode is null)
            return NotFound(new ProblemDetails { Title = "PromoCode not found", Detail = $"PromoCode with Id {id} not found." });

        var customer = await customerRepository.GetById(request.CustomerId, true, ct);
        if (customer is null)
            return BadRequest(new ProblemDetails { Title = "Invalid customer", Detail = $"Customer with Id {request.CustomerId} not found." });

        var customerPromoCode = customer.CustomerPromoCodes.FirstOrDefault(cp => cp.PromoCodeId == id);
        if (customerPromoCode is null)
            return BadRequest(new ProblemDetails { Title = "PromoCode not assigned", Detail = "This promo code was not assigned to the customer." });

        if (customerPromoCode.AppliedAt is not null)
            return BadRequest(new ProblemDetails { Title = "Already applied", Detail = "This promo code has already been used." });

        customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;

        try
        {
            await customerRepository.Update(customer, ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}