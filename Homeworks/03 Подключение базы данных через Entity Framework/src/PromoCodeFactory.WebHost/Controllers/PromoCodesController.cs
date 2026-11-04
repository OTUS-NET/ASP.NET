using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(IRepository<PromoCode> promoCodeRepository,
    IRepository<Employee> employeeRepository,
    IRepository<Preference> preferenceRepository) : BaseController
{
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        try
        {
            IReadOnlyCollection<PromoCode> data = await promoCodeRepository.GetAll(withIncludes: false, ct);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Получить промокод по id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            PromoCode? item = await promoCodeRepository.GetById(id, withIncludes: false, ct);
            if (item is null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
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
        try
        {
            Employee? manager = await employeeRepository.GetById(request.PartnerManagerId, withIncludes: false, ct);
            if (manager is null)
            {
                return NotFound($"Partner manager with id {request.PartnerManagerId} not found");
            }

            Preference? preference = await preferenceRepository.GetById(request.PreferenceId, withIncludes: false, ct);

            if (preference is null)
            {
                return NotFound($"Preference with id {request.PreferenceId} not found");
            }

            var promoCode = new PromoCode
            {
                Code = request.Code,
                ServiceInfo = request.ServiceInfo,
                BeginDate = request.BeginDate,
                EndDate = request.EndDate,
                PartnerName = request.PartnerName,
                PartnerManager = manager,
                Preference = preference
            };

            await promoCodeRepository.Add(promoCode, ct);

            var response = PromoCodesMapper.ToPromoCodeShortResponse(promoCode);

            return Created($"api/v1/[controller]/Create", response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
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
        throw new NotImplementedException();
    }
}
