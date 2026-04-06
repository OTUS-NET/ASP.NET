using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Customers;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController(
    IRepository<Customer> customerRepository,
    IRepository<Preference> preferenceRepository,
    IRepository<PromoCode> promoCodeRepository//,
    //IRepository<CustomerPromoCode> customerPromoCodeRepository
) : BaseController
{
    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var customers = await customerRepository.GetAll(withIncludes: true, ct: ct);
        var models = customers.Select(CustomersMapper.ToCustomerShortResponse).ToList();
        return Ok(models);
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await customerRepository.GetById(id, withIncludes: true, ct: ct);
        if (customer == null)
            return NotFound();

        var promoCodeIds = customer.CustomerPromoCodes.Select(x => x.PromoCodeId).Distinct().ToList();

        var promoCodes = promoCodeIds.Count == 0
            ? Array.Empty<PromoCode>()
            : await promoCodeRepository.GetByRangeId(promoCodeIds, withIncludes: true, ct: ct);

        var promoCodeById = promoCodes.ToDictionary(x => x.Id, x => x);

        var promoCodeResponses = customer.CustomerPromoCodes
            .Where(link => promoCodeById.ContainsKey(link.PromoCodeId))
            .Select(link => CustomerPromoCodesMapper.ToCustomerPromoCodeResponse(promoCodeById[link.PromoCodeId], link))
            .ToList();

        var response = new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToList(),
            promoCodeResponses);

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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var preferences = await preferenceRepository.GetByRangeId(request.PreferenceIds, withIncludes: false, ct: ct);
        if (preferences.Count != request.PreferenceIds.Distinct().Count())
            return BadRequest("PreferenceId не найдены.");

        var customer = CustomersMapper.ToCustomer(request, preferences);

        await customerRepository.Add(customer, ct);

        var response = CustomersMapper.ToCustomerShortResponse(customer);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, response);
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = await customerRepository.GetById(id, withIncludes: true, ct: ct);
        if (customer == null)
            return NotFound();

        var preferences = await preferenceRepository.GetByRangeId(request.PreferenceIds, withIncludes: false, ct: ct);
        if (preferences.Count != request.PreferenceIds.Distinct().Count())
            return BadRequest("PreferenceId не найдены.");

        CustomersMapper.ApplyUpdate(customer, request, preferences);

        try
        {
            await customerRepository.Update(customer, ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return Ok(CustomersMapper.ToCustomerShortResponse(customer));
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
