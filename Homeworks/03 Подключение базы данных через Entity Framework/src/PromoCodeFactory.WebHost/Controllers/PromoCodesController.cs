using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(
        IRepository<PromoCode> promoCodeRepository,
        IRepository<Employee> employeeRepository,
        IRepository<Preference> preferenceRepository,
        IRepository<Customer> customerRepository,
        IRepository<CustomerPromoCode> customerPromoCodeRepository
    ) : BaseController
{
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var promoCodes = await promoCodeRepository.GetAll(withIncludes: true, ct: ct);
        var response = promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse).ToList();
        return Ok(response);
    }
    
    /// <summary>
    /// Получить промокод по id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promoCode = await promoCodeRepository.GetById(id, withIncludes: true, ct: ct);
        if (promoCode == null) return NotFound();
        return Ok(PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
    }
    
    /// <summary>
    /// Создать промокод и выдать его клиентам с указанным предпочтением
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> Create(PromoCodeCreateRequest request, CancellationToken ct)
    {
        // 1. Проверяем, что партнёрский менеджер существует
        var partnerManager = await employeeRepository.GetById(request.PartnerManagerId, ct: ct);
        if (partnerManager == null) return BadRequest("Partner manager not found.");

        // 2. Проверяем, что предпочтение существует
        var preference = await preferenceRepository.GetById(request.PreferenceId, ct: ct);
        if (preference == null) return BadRequest("Preference not found.");

        // 3. Создаём промокод в памяти (навигации присваиваем объектами, не Id)
        var promoCode = new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            ServiceInfo = request.ServiceInfo,
            PartnerName = request.PartnerName,
            BeginDate = request.BeginDate,
            EndDate = request.EndDate,
            PartnerManager = partnerManager,
            Preference = preference
        };

        // 4. Ищем клиентов с указанным предпочтением
        //    withIncludes не нужен: фильтрация по Preferences. Any() транслируется в JOIN/EXISTS в SQL
        var customers = await customerRepository.GetWhere(
            c => c.Preferences.Any(p => p.Id == request.PreferenceId),
            ct: ct);

        // 5. Создаём связки "клиент-промокод" (выдача промокода)
        var customerPromoCodes = customers
            .Select(c => new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = c.Id,
                PromoCodeId = promoCode.Id,
                CreatedAt = DateTimeOffset.UtcNow
            })
            .ToList();

        // 6. Сохраняем атомарно: промокод + все выдачи в одной транзакции
        await promoCodeRepository.ExecuteInTransaction(async () =>
        {
            await promoCodeRepository.Add(promoCode, ct);
            foreach (var cpc in customerPromoCodes)
                await customerPromoCodeRepository.Add(cpc, ct);
        }, ct);

        // 7. Возвращаем 201 Created + Location на GetById
        return CreatedAtAction(
            nameof(GetById),
            new { id = promoCode.Id },
            PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
    }

    /// <summary>
    /// Применить промокод (отметить, что клиент использовал промокод)
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
        // 1. Ищем связку "клиент-промокод" по PromoCodeId и CustomerId
        var customerPromoCodes = await customerPromoCodeRepository.GetWhere(
            cpc => cpc.PromoCodeId == id && cpc.CustomerId == request.CustomerId,
            ct: ct);

        // 2. Если связки нет — клиенту этот промокод не выдавался (или уже удалён)
        var customerPromoCode = customerPromoCodes.FirstOrDefault();
        if (customerPromoCode == null) return NotFound();

        // 3. Отмечаем использование: фиксируем дату применения
        customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;

        // 4. Сохраняем изменения
        await customerPromoCodeRepository.Update(customerPromoCode, ct);

        // 5. 204 No Content — успешно, тело не нужно
        return NoContent();
    }
}
