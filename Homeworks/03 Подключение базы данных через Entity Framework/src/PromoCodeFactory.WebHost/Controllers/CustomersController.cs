using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Customers;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Контроллер для работы с клиентами.
/// Предоставляет методы чтения, создания, обновления и удаления клиентов.
/// </summary>
public class CustomersController(
    IRepository<Customer> customersRepository,
    IRepository<Preference> preferencesRepository,
    IRepository<PromoCode> promoCodesRepository) : BaseController
{
    /// <summary>
    /// Get - возвращает список всех клиентов.
    /// Клиенты загружаются вместе с предпочтениями, чтобы сразу сформировать CustomerShortResponse.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var customers = await customersRepository.GetAll(withIncludes: true, ct: ct);
        return Ok(customers.Select(CustomersMapper.ToCustomerShortResponse).ToList());
    }

    /// <summary>
    /// GetById - возвращает клиента по идентификатору.
    /// В ответ включаются предпочтения клиента и выданные ему промокоды.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await customersRepository.GetById(id, withIncludes: true, ct: ct);
        if (customer is null)
            return NotFound();

        // CustomerPromoCode хранит только PromoCodeId, поэтому данные самих промокодов загружаются отдельным запросом.
        var promoCodeIds = customer.CustomerPromoCodes.Select(customerPromoCode => customerPromoCode.PromoCodeId);
        var promoCodes = await promoCodesRepository.GetByRangeId(promoCodeIds, withIncludes: true, ct: ct);

        return Ok(CustomersMapper.ToCustomerResponse(customer, promoCodes));
    }

    /// <summary>
    /// Create - создает нового клиента.
    /// Перед сохранением проверяет, что все переданные PreferenceIdы существуют в базе данных.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        var preferences = await GetPreferences(request.PreferenceIds, ct);
        if (preferences.Count != request.PreferenceIds.Distinct().Count())
            return BadRequest(new ProblemDetails { Title = "Invalid preferences", Detail = "One or more preferences were not found." });

        var customer = CustomersMapper.ToCustomer(request, preferences);
        await customersRepository.Add(customer, ct);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, CustomersMapper.ToCustomerShortResponse(customer));
    }

    /// <summary>
    /// Update - обновляет данные клиента и его предпочтения.
    /// Если клиент не найден, возвращает 404; если указано несуществующее предпочтение, возвращает 400.
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
        var customer = await customersRepository.GetById(id, withIncludes: true, ct: ct);
        if (customer is null)
            return NotFound();

        var preferences = await GetPreferences(request.PreferenceIds, ct);
        if (preferences.Count != request.PreferenceIds.Distinct().Count())
            return BadRequest(new ProblemDetails { Title = "Invalid preferences", Detail = "One or more preferences were not found." });

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Preferences = preferences.ToList();

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
    /// Delete - удаляет клиента по идентификатору.
    /// Если клиент не найден, возвращает 404.
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

    /// <summary>
    /// GetPreferences - загружает предпочтения по списку идентификаторов.
    /// Distinct убирает дубликаты, чтобы корректно сравнить количество найденных записей с количеством запрошенных.
    /// </summary>
    private async Task<IReadOnlyCollection<Preference>> GetPreferences(IEnumerable<Guid> preferenceIds, CancellationToken ct)
    {
        return await preferencesRepository.GetByRangeId(preferenceIds.Distinct(), ct: ct);
    }
}
