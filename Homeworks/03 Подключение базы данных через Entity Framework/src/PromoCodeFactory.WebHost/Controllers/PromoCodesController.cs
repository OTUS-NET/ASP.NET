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
        var models = promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse).ToList();
        return Ok(models);
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
        if (promoCode == null)
            return NotFound();

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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var partnerManager = await employeeRepository.GetById(request.PartnerManagerId, withIncludes: true, ct: ct);
        if (partnerManager == null)
            return NotFound($"PartnerManager с id '{request.PartnerManagerId}' не найден.");

        var preference = await preferenceRepository.GetById(request.PreferenceId, withIncludes: false, ct: ct);
        if (preference == null)
            return NotFound($"Preference с id '{request.PreferenceId}' не найден.");

        if (request.EndDate < request.BeginDate)
            return BadRequest("Дата окончания должна быть бюольше либо равна дате начала.");

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

        await promoCodeRepository.Add(promoCode, ct);

        // Выдаём промокод клиентам с указанным предпочтением
        var customers = await customerRepository.GetWhere(
            c => c.Preferences.Any(p => p.Id == request.PreferenceId),
            withIncludes: false,
            ct: ct);

        foreach (var customer in customers)
        {
            var link = new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                PromoCodeId = promoCode.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                AppliedAt = null
            };

            await customerPromoCodeRepository.Add(link, ct);
        }

        return CreatedAtAction(nameof(GetById), new { id = promoCode.Id }, PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // промокод существует
        var promoCode = await promoCodeRepository.GetById(id, withIncludes: false, ct: ct);
        if (promoCode == null)
            return NotFound();

        // клиент существует
        var customer = await customerRepository.GetById(request.CustomerId, withIncludes: false, ct: ct);
        if (customer == null)
            return NotFound();

        var links = await customerPromoCodeRepository.GetWhere(
            x => x.CustomerId == request.CustomerId && x.PromoCodeId == id,
            withIncludes: false,
            ct: ct);

        var link = links.FirstOrDefault();
        if (link == null)
            return NotFound();

        if (link.AppliedAt != null)
            return BadRequest("Промо код уже был применен к данному пользователю.");

        link.AppliedAt = DateTimeOffset.UtcNow;

        try
        {
            await customerPromoCodeRepository.Update(link, ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}
