using Microsoft.AspNetCore.Mvc;
using Pcf.Common.Web.Controllers;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mapping;
using Pcf.GivingToCustomer.WebHost.Models.Customers;

namespace Pcf.GivingToCustomer.WebHost.Controllers;

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
        var customers = await customersRepository.GetAll(ct: ct);
        var preferences = await PreferencesById(ct);
        return Ok(customers.Select(c => CustomersMapper.ToCustomerShortResponse(c, preferences)));
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await customersRepository.GetById(id, ct: ct);
        if (customer is null)
            return NotFound();

        var preferences = await PreferencesById(customer.PreferenceIds, ct);
        var promoCodeIds = customer.CustomerPromoCodes.Select(cpc => cpc.PromoCodeId).Distinct().ToArray();
        var promocodes = await promoCodesRepository.GetByRangeId(promoCodeIds, ct: ct);

        return Ok(CustomersMapper.ToCustomerResponse(customer, preferences, promocodes));
    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        var (preferences, missingPreferenceId) = await LoadPreferences(request.PreferenceIds, ct);
        if (preferences is null)
            return InvalidPreference(missingPreferenceId!.Value);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PreferenceIds = request.PreferenceIds.ToList()
        };

        await customersRepository.Add(customer, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            CustomersMapper.ToCustomerShortResponse(customer, preferences));
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
        var customer = await customersRepository.GetById(id, ct: ct);
        if (customer is null)
            return NotFound();

        var (preferences, missingPreferenceId) = await LoadPreferences(request.PreferenceIds, ct);
        if (preferences is null)
            return InvalidPreference(missingPreferenceId!.Value);

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.PreferenceIds = request.PreferenceIds.ToList();

        try
        {
            await customersRepository.Update(customer, ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return Ok(CustomersMapper.ToCustomerShortResponse(customer, preferences));
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

    private async Task<(IReadOnlyDictionary<Guid, Preference>? Preferences, Guid? MissingId)> LoadPreferences(
        IReadOnlyCollection<Guid> preferenceIds,
        CancellationToken ct)
    {
        var preferences = new Dictionary<Guid, Preference>();
        foreach (var preferenceId in preferenceIds)
        {
            var preference = await preferencesRepository.GetById(preferenceId, false, ct);
            if (preference is null)
                return (null, preferenceId);

            preferences[preference.Id] = preference;
        }

        return (preferences, null);
    }

    private BadRequestObjectResult InvalidPreference(Guid missingId)
    {
        return BadRequest(new ProblemDetails
        {
            Title = "Invalid preference",
            Detail = $"Preference with Id {missingId} not found."
        });
    }

    private async Task<IReadOnlyDictionary<Guid, Preference>> PreferencesById(CancellationToken ct)
    {
        var preferences = await preferencesRepository.GetAll(ct: ct);
        return preferences.ToDictionary(p => p.Id);
    }

    private async Task<IReadOnlyDictionary<Guid, Preference>> PreferencesById(
        IEnumerable<Guid> preferenceIds,
        CancellationToken ct)
    {
        var preferences = new Dictionary<Guid, Preference>();
        foreach (var preferenceId in preferenceIds)
        {
            var preference = await preferencesRepository.GetById(preferenceId, false, ct);
            if (preference is not null)
                preferences[preference.Id] = preference;
        }

        return preferences;
    }
}
