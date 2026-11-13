using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController(IRepository<Customer> customerRepository, IRepository<PromoCode> promoCodeRepository, IRepository<Preference> preferenceRepository) : BaseController
{
    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>?> Get(CancellationToken ct)
    {
        try
        {
            IEnumerable<CustomerShortResponse> items = (await customerRepository.GetAll(true, ct))
                .Select(x => new CustomerShortResponse(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    x.Preferences.Select(y => PreferencesMapper.ToPreferenceShortResponse(y)).ToArray()
                    ));

            return Ok(items);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            Customer? item = await customerRepository.GetById(id, true, ct);

            if (item is null)
            {
                return NotFound();
            }

            IReadOnlyCollection<PromoCode> promoCodes = await promoCodeRepository.GetByRangeId(
                item.CustomerPromoCodes.Select(x => x.PromoCodeId),
                withIncludes: true,
                ct);


            var promoCodesById = promoCodes.ToDictionary(x => x.Id);
            IReadOnlyCollection<CustomerPromoCodeResponse> promoCodeResponses = item.CustomerPromoCodes
                .Select(link =>
                {
                    PromoCode promoCode = promoCodesById[link.PromoCodeId];
                    return new CustomerPromoCodeResponse(
                        promoCode.Id,
                        promoCode.Code,
                        promoCode.ServiceInfo,
                        promoCode.PartnerName,
                        promoCode.BeginDate,
                        promoCode.EndDate,
                        promoCode.PartnerManager.Id,
                        promoCode.Preference.Id,
                        link.CreatedAt,
                        link.AppliedAt);
                })
                .ToArray();

            var responce = new CustomerResponse(
                item.Id,
                item.FirstName,
                item.LastName,
                item.Email,
                item.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToArray(),
                promoCodeResponses);

            return Ok(responce);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("Create")]
    public async Task<ActionResult> Create(CustomerCreateRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        try
        {
            IReadOnlyCollection<Preference> preference = await preferenceRepository.GetByRangeId(request.PreferenceIds, false, ct);

            var customer = new Customer
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Preferences = preference.ToArray()
            };
            await customerRepository.Add(customer, ct);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> Update(Guid id, CustomerUpdateRequest updateRequest, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        try
        {
            IReadOnlyCollection<Preference> preference = await preferenceRepository.GetByRangeId(updateRequest.PreferenceIds, false, ct);
            var customer = await customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.FirstName = updateRequest.FirstName;
            customer.LastName = updateRequest.LastName;
            customer.Email = updateRequest.Email;
            customer.Preferences = preference.ToArray();

            await customerRepository.Update(customer, ct);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        try
        {
            await customerRepository.Delete(id, ct);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
