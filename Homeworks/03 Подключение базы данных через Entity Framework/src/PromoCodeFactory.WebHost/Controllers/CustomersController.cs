using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Customers;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController(IRepository<Customer> customerRepository,
    IRepository<PromoCode> promoCodeRepository,
    IRepository<Preference> preferenceRepository) : BaseController
{
    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var customers = await customerRepository.GetAll(true, ct);
        var customersModels = customers.Select(CustomersMapper.ToCustomerShortResponse);

        return Ok(customersModels);
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await customerRepository.GetById(id, true, ct);

        if (customer == null)
            return NotFound();

        var customerPromoCodesIds = customer.CustomerPromoCodes.Select(x => x.PromoCodeId);
        var promoCodes = await promoCodeRepository.GetByRangeId(customerPromoCodesIds, true, ct);

        var customerModel = CustomersMapper.ToCustomerResponse(customer, promoCodes);

        return Ok(customerModel);
    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        var uniqPreferenceIds = request.PreferenceIds.Distinct().ToList();
        if (uniqPreferenceIds.Count() != request.PreferenceIds.Count())
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid preferences",
                Detail = "There are duplicate preferences"
            });

        var preferences = await preferenceRepository.GetByRangeId(uniqPreferenceIds, ct: ct);
        if (preferences.Count != uniqPreferenceIds.Count)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid preferences",
                Detail = "Some preferences are not found"
            });

        var customer = CustomersMapper.ToCustomer(request, preferences);
        await customerRepository.Add(customer, ct);

        var customerModel = CustomersMapper.ToCustomerShortResponse(customer);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customerModel);
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
        var customer = await customerRepository.GetById(id, true, ct);
        if (customer == null)
            return NotFound();

        var uniqPreferenceIds = request.PreferenceIds.Distinct().ToList();
        if (uniqPreferenceIds.Count() != request.PreferenceIds.Count())
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid preferences",
                Detail = "There are duplicate preferences"
            });

        var preferences = await preferenceRepository.GetByRangeId(uniqPreferenceIds, ct: ct);
        if (preferences.Count != uniqPreferenceIds.Count)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid preferences",
                Detail = "Some preferences are not found"
            });

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Preferences = preferences.ToList();

        await customerRepository.Update(customer, ct);

        var customerModel = CustomersMapper.ToCustomerShortResponse(customer);

        return Ok(customerModel);
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
            await customerRepository.Delete(id, ct: ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}
