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
        // 1. Загрузить всех клиентов вместе с Preferences (withIncludes: true)
        var customers = await customerRepository.GetAll(withIncludes: true, ct: ct);

        // 2. Преобразовать в DTO
        var response = customers.Select(CustomersMapper.ToCustomerShortResponse).ToList();

        // 3. Вернуть 200 OK
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
        var customer = await customerRepository.GetById(id, withIncludes: true, ct: ct);
        if (customer == null) return NotFound();

        // Собрать Id промокодов из CustomerPromoCodes
        var promoCodeIds = customer.CustomerPromoCodes.Select(cpc => cpc.PromoCodeId).ToList();

        // Загрузить сами промокоды (с Include на PartnerManager/Preference — через withIncludes)
        var promoCodes = await promoCodeRepository.GetByRangeId(promoCodeIds, withIncludes: true, ct: ct);
        var promoCodesById = promoCodes.ToDictionary(p => p.Id);

        var response = CustomersMapper.ToCustomerResponse(customer, promoCodesById);
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
        // 1. Загрузить Preferences по Id одним запросом (вместо N запросов)
        var preferences = await preferenceRepository.GetByRangeId(request.PreferenceIds, withIncludes: false, ct: ct);
        if (preferences.Count != request.PreferenceIds.Length)
        {
            return BadRequest("Some preferences not found.");
        }

        // 2. Создать сущность Customer
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Preferences = [.. preferences]
        };

        // 3. Сохранить в БД
        await customerRepository.Add(customer, ct);

        // 4. Вернуть 201 + Location (URL на GetById)
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, CustomersMapper.ToCustomerShortResponse(customer));
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
        // 1. Найти клиента с Preferences (withIncludes: true)
        var customer = await customerRepository.GetById(id, withIncludes: true, ct: ct);
        if (customer == null) return NotFound();

        // 2. Загрузить Preferences по request.PreferenceIds
        var preferences = await preferenceRepository.GetByRangeId(request.PreferenceIds, withIncludes: false, ct: ct);
        if (preferences.Count != request.PreferenceIds.Length) return BadRequest("Some preferences not found.");

        // 3. Обновить поля
        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;

        // 4. Заменить коллекцию Preferences
        customer.Preferences.Clear();
        foreach (var pref in preferences) customer.Preferences.Add(pref);

        // 5. Сохранить
        await customerRepository.Update(customer, ct);

        // 6. Вернуть
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
        // 1. Проверить, что клиент существует
        var customer = await customerRepository.GetById(id, withIncludes: false, ct: ct);
        if (customer == null) return NotFound();

        // 2. Удалить
        try
        {
            await customerRepository.Delete(id, ct);
        }
        catch (EntityNotFoundException)
        {
            // Защита от гонки: клиент мог быть удалён между GetById и Delete
            return NotFound();
        }

        // 3. Вернуть 204 No Content (успешное удаление)
        return NoContent();
    }
}
