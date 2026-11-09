using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.Preferences;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController : BaseController
{
    private IRepository<Customer> _customerRepository;
    private IRepository<Preference> _preferenceRepository;

    public CustomersController(IRepository<Customer> customerRepository, IRepository<Preference> preferenceRepository)
    {
        _customerRepository = customerRepository;
        _preferenceRepository = preferenceRepository;
    }

    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var customers = await _customerRepository.GetAll(true, ct);
        var data = customers.Select(c => new CustomerShortResponse
        (
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email,
            c.Preferences.Select(p => new PreferenceShortResponse(p.Id, p.Name)).ToList()
        ));
        return Ok(data);
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var data = Get(id, ct);
        return Ok(data);
    }

    private async Task<CustomerShortResponse> Get(Guid id, CancellationToken ct)
    {
        var customer = await _customerRepository.GetById(id, true, ct);
        var data = new CustomerShortResponse
        (
            customer!.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Preferences.Select(p => new PreferenceShortResponse(p.Id, p.Name)).ToList()
        );
        return data;
    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        Guid newId = Guid.NewGuid();
        var preferences = await _preferenceRepository.GetByRangeId(request.PreferenceIds, false, ct);
        var customer = new Customer
        {
            Id = newId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Preferences = preferences.Select(p => new Preference { Id = p.Id, Name = p.Name }).ToList()
        };
        await _customerRepository.Add(customer, ct);
        var data = Get(newId, ct);
        return Ok(data);
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
        var customer = await _customerRepository.GetById(id, false, ct);
        if (customer == null)
            throw new EntityNotFoundException(typeof(Customer), id);

        var preferences = await _preferenceRepository.GetByRangeId(request.PreferenceIds, false, ct);

        customer.Email = request.Email;
        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Preferences = preferences.Select(p => new Preference { Id = p.Id, Name = p.Name }).ToList();

        await _customerRepository.Update(customer, ct);
        var data = Get(id, ct);
        return Ok(data);
    }

    /// <summary>
    /// Удалить клиента
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _customerRepository.Delete(id, ct);
        return Ok();
    }
}
