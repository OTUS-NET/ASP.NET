using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Customers;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController(
    IRepository<Customer> customersRepository,
    IRepository<Preference> preferencesRepository,
    IRepository<PromoCode> promoCodesRepository)
    : BaseController
{
    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var customers = await customersRepository.GetAll(false, ct);
        return Ok(customers.Select(CustomersMapper.ToCustomerShortResponse));
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await customersRepository.GetById(id, withIncludes: true, ct);
        if (customer is null)
            return NotFound();

        var promoCodeIds = customer.CustomerPromoCodes.Select(cpc => cpc.PromoCodeId).Distinct().ToArray();
        var promocodes = await promoCodesRepository.GetByRangeId(promoCodeIds, ct: ct);

        return Ok(CustomersMapper.ToCustomerResponse(customer, promocodes));
    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        var preferences = new List<Preference>();
        foreach (var preferenceId in request.PreferenceIds)
        {
            var preference = await preferencesRepository.GetById(preferenceId, false, ct);
            if (preference is null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid preference",
                    Detail = $"Preference with Id {preferenceId} not found."
                });
            preferences.Add(preference);
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Preferences = preferences
        };

        await customersRepository.Add(customer, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            CustomersMapper.ToCustomerShortResponse(customer));
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
        var customer = await customersRepository.GetById(id, withIncludes: true, ct);
        if (customer is null)
            return NotFound();

        var preferences = new List<Preference>();
        foreach (var preferenceId in request.PreferenceIds)
        {
            var preference = await preferencesRepository.GetById(preferenceId, false, ct);
            if (preference is null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid preference",
                    Detail = $"Preference with Id {preferenceId} not found."
                });
            preferences.Add(preference);
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Preferences = preferences;

        try
        {
            await customersRepository.Update(customer, ct);
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
            await customersRepository.Delete(id, ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}
