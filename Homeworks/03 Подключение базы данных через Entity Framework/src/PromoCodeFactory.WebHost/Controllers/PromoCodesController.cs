using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Контроллер для работы с промокодами.
/// Отвечает за чтение промокодов, создание промокода, выдачу клиентам и отметку применения.
/// </summary>
public class PromoCodesController(
    IRepository<PromoCode> promoCodesRepository,
    IRepository<Customer> customersRepository,
    IRepository<CustomerPromoCode> customerPromoCodesRepository,
    IRepository<Employee> employeesRepository,
    IRepository<Preference> preferencesRepository) : BaseController
{
    /// <summary>
    /// Get - возвращает список всех промокодов.
    /// Промокоды загружаются вместе с менеджером-партнером и предпочтением.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var promoCodes = await promoCodesRepository.GetAll(withIncludes: true, ct: ct);
        return Ok(promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse).ToList());
    }

    /// <summary>
    /// GetById - возвращает промокод по идентификатору.
    /// Если промокод не найден, возвращает 404.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promoCode = await promoCodesRepository.GetById(id, withIncludes: true, ct: ct);
        if (promoCode is null)
            return NotFound();

        return Ok(PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
    }

    /// <summary>
    /// Create - создает промокод и выдает его клиентам с указанным предпочтением.
    /// Сначала проверяет существование менеджера-партнера и предпочтения, затем создает PromoCode
    /// и отдельные CustomerPromoCode-записи для всех подходящих клиентов.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> Create(PromoCodeCreateRequest request, CancellationToken ct)
    {
        var partnerManager = await employeesRepository.GetById(request.PartnerManagerId, ct: ct);
        if (partnerManager is null)
            return BadRequest(new ProblemDetails { Title = "Invalid partner manager", Detail = $"Employee with Id {request.PartnerManagerId} not found." });

        var preference = await preferencesRepository.GetById(request.PreferenceId, ct: ct);
        if (preference is null)
            return BadRequest(new ProblemDetails { Title = "Invalid preference", Detail = $"Preference with Id {request.PreferenceId} not found." });

        var promoCode = PromoCodesMapper.ToPromoCode(request, partnerManager, preference);
        await promoCodesRepository.Add(promoCode, ct);

        // Выбираем только тех клиентов, у которых есть предпочтение, указанное при создании промокода.
        var customers = await customersRepository.GetWhere(
            customer => customer.Preferences.Any(customerPreference => customerPreference.Id == request.PreferenceId),
            withIncludes: true,
            ct: ct);

        foreach (var customer in customers)
        {
            // CustomerPromoCode фиксирует факт выдачи промокода конкретному клиенту.
            // AppliedAt остается null, пока клиент не применит промокод через Apply.
            await customerPromoCodesRepository.Add(
                new CustomerPromoCode
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    PromoCodeId = promoCode.Id,
                    CreatedAt = DateTimeOffset.UtcNow,
                    AppliedAt = null
                },
                ct);
        }

        return CreatedAtAction(nameof(GetById), new { id = promoCode.Id }, PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
    }

    /// <summary>
    /// Apply - отмечает, что клиент применил выданный ему промокод.
    /// Метод ищет запись выдачи по PromoCodeId и CustomerId, затем заполняет AppliedAt текущим временем.
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
        var promoCode = await promoCodesRepository.GetById(id, ct: ct);
        if (promoCode is null)
            return NotFound();

        var customerPromoCode = (await customerPromoCodesRepository.GetWhere(
                item => item.PromoCodeId == id && item.CustomerId == request.CustomerId,
                ct: ct))
            .FirstOrDefault();

        if (customerPromoCode is null)
            return NotFound();

        if (customerPromoCode.AppliedAt is not null)
            return BadRequest(new ProblemDetails { Title = "Promo code already applied" });

        customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;
        await customerPromoCodesRepository.Update(customerPromoCode, ct);

        return NoContent();
    }
}
