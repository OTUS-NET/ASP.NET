using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController : BaseController
{
    private IRepository<PromoCode> _promoCodeRepository;
    private IRepository<Preference> _preferenceRepository;
    private IRepository<Employee> _employeeRepository;
    private IRepository<CustomerPromoCode> _customerPromoCodeRepository;

    public PromoCodesController(IRepository<PromoCode> promoCodeRepository, IRepository<Preference> preferenceRepository, IRepository<Employee> employeeRepository, IRepository<CustomerPromoCode> customerPromoCodeRepository)
    {
        _promoCodeRepository = promoCodeRepository;
        _preferenceRepository = preferenceRepository;
        _employeeRepository = employeeRepository;
        _customerPromoCodeRepository = customerPromoCodeRepository;
    }

    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var data = (await _promoCodeRepository.GetAll(true, ct)).Select(x => new PromoCodeShortResponse
        (
            x.Id,
            x.Code,
            x.ServiceInfo,
            x.PartnerName,
            x.BeginDate,
            x.EndDate,
            x.PartnerManager.Id,
            x.Preference.Id
        ));
        return Ok(data);
    }

    /// <summary>
    /// Получить промокод по id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promoCode = await _promoCodeRepository.GetById(id, false, ct);
        var data = new PromoCodeShortResponse
        (
            promoCode!.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.PartnerName,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.PartnerManager.Id,
            promoCode.Preference.Id
        );
        return Ok(data);
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
        var partner = await _employeeRepository.GetById(request.PartnerManagerId, false, ct);
        if (partner == null) return BadRequest("Partner not found.");

        var preference = await _preferenceRepository.GetById(request.PreferenceId, false, ct);
        if (preference == null) return BadRequest("Preference not found.");

        Guid newId = Guid.NewGuid();
        var promoCode = new PromoCode
        {
            Id = newId,
            Code = request.Code,
            BeginDate = request.BeginDate,
            EndDate = request.EndDate,
            PartnerName = request.PartnerName,
            ServiceInfo = request.ServiceInfo,
            PartnerManager = partner,
            Preference = preference
        };
        promoCode = await _promoCodeRepository.GetById(newId, true, ct);
        var data = new PromoCodeShortResponse
        (
            promoCode!.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.PartnerName,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.PartnerManager.Id,
            promoCode.Preference.Id
        );
        return Ok(data);
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
        var customerPromoCode = (await _customerPromoCodeRepository.GetWhere(_ => _.PromoCodeId == id && _.CustomerId == request.CustomerId, false, ct)).FirstOrDefault();
        if (customerPromoCode == null) return NotFound();
        customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;
        await _customerPromoCodeRepository.Update(customerPromoCode, ct);
        return Ok();
    }
}
