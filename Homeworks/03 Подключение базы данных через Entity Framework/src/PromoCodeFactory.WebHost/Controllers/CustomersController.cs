using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.Preferences;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController(IRepository<Customer> customerRepository, IRepository<PromoCode> promoCodeRepository, IRepository<Preference> preferenceRepository) : BaseController
{
    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var customers = await customerRepository.GetAll(withIncludes: false, ct);

        var response = customers.Select(c => new CustomerShortResponse(
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email,
            c.Preferences.Select(p => new PreferenceShortResponse(
                p.Id,
                p.Name
            )).ToList()
        )).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {

        var customer = await customerRepository.GetById(id, withIncludes: false, ct);

        if (customer == null)
            return NotFound();

        var promoCodesIds = customer.CustomerPromoCodes.Select(p => p.Id).ToList();

        var promoCodes = await promoCodeRepository.GetByRangeId(promoCodesIds, withIncludes: false, ct);

        var promoCodeResponses = customer.CustomerPromoCodes.Select(cp => {

        var promo = promoCodes.FirstOrDefault(p => p.Id == cp.PromoCodeId);

         return new CustomerPromoCodeResponse(
             cp.PromoCodeId,
             promo?.Code ?? "",
             promo?.ServiceInfo ?? "",
             promo?.PartnerName ?? "",
             promo?.BeginDate ?? DateTimeOffset.MinValue,
             promo?.EndDate ?? DateTimeOffset.MaxValue,
             promo?.PartnerManager?.Id ?? Guid.Empty,
             promo?.Preference?.Id ?? Guid.Empty,     
             cp.CreatedAt,
             cp.AppliedAt
            );
        }).ToList();

        var response = new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(p => new PreferenceShortResponse(p.Id, p.Name)).ToList(),
            promoCodeResponses
        );

        return Ok(response);

    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        try
        {
            var preferences = await preferenceRepository.GetByRangeId(request.PreferenceIds, ct: ct);

            await customerRepository.Add(new Customer()
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Preferences = preferences.ToList(),
                CustomerPromoCodes = new List<CustomerPromoCode>()
            }, ct);
            return Ok();
        }

        catch(BadHttpRequestException)
        {
            return BadRequest();
        }

    }

    /// <summary>
    /// Обновить клиента
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerShortResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] CustomerUpdateRequest request,
        CancellationToken ct)
    {
        var customer = await customerRepository.GetById(id, withIncludes: false, ct);

        if (customer == null)
            return NotFound();

        var newPreferences = await preferenceRepository.GetByRangeId(request.PreferenceIds, ct: ct);

        var customerPromoCodes = customer.CustomerPromoCodes;

        try
        {
            await customerRepository.Update(new Customer()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Preferences = newPreferences.ToList().Count > 0 ? newPreferences.ToList() : new List<Preference>(),
                CustomerPromoCodes = customerPromoCodes
            }, ct);

            return Ok();
        }
        catch (BadHttpRequestException)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Удалить клиента
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await customerRepository.Delete(id, ct);

            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }
}
