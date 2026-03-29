using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Customers;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController(
    IRepository<Customer> customerRepository,
    IRepository<Preference> preferenceRepository,
    IRepository<PromoCode> promoCodeRepository
    ) : BaseController
{
    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var customer = await customerRepository.GetAll(ct: ct);
        return Ok(customer.Select(CustomersMapper.ToCustomerShortResponse));
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

        if (customer is null)
        {
            return NotFound(new ProblemDetails { Title = "Invalid Id", Detail = $"Customer with Id {id} not found." });
        }
        var customerRelatedPromoCodes = await promoCodeRepository.GetByRangeId(customer.CustomerPromoCodes.Select(cpc => cpc.PromoCodeId), ct: ct);
        var customerResponse = CustomersMapper.ToCustomerResponse(
            customer,
            customer.CustomerPromoCodes.Join(customerRelatedPromoCodes, cpc => cpc.PromoCodeId, crpc => crpc.Id, (cpc, crpc) => (cpc, crpc))
            );
        return Ok(customerResponse);
    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        var preferences = await preferenceRepository.GetByRangeId(request.PreferenceIds, ct: ct);

        if (preferences.Count == 0)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid preference Ids", Detail = $"No preference was found." });
        }
        else
        {
            var customer = CustomersMapper.ToCustomer(request, preferences);
            await customerRepository.Add(customer, ct);
            return Created(customer.Id.ToString(), CustomersMapper.ToCustomerShortResponse(customer));
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
        var customer = await customerRepository.GetById(id, ct: ct);
        var preferences = await preferenceRepository.GetByRangeId(request.PreferenceIds, ct: ct);

        if (customer is null)
        {
            return NotFound(new ProblemDetails { Title = "Invalid customer", Detail = $"Customer with Id {id} not found." });
        }
        if (preferences.Count == 0)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid preference Ids", Detail = $"No preference was found." });
        }
        var updatedCustomer = CustomersMapper.ToCustomer(request, preferences);
        await customerRepository.Update(updatedCustomer, ct);
        return Ok(CustomersMapper.ToCustomerShortResponse(updatedCustomer));
    }

    /// <summary>
    /// Удалить клиента
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var customerToDelete = await customerRepository.GetById(id, ct: ct);

        if (customerToDelete is null)
        {
            return NotFound(new ProblemDetails { Title = "Invalid customer", Detail = $"Customer with Id {id} not found." });
        }
        await customerRepository.Delete(id, ct);
        return NoContent();
    }
}
