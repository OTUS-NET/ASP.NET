using MediatR;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Commands.Commands.PromoCodes;
using PromoCodeFactory.Commands.Queries.PromoCodes;
using PromoCodeFactory.Contracts.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Контроллер для управления предпочтениями
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PromoCodesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Выдать промокоды клиентам с определенным предпочтением
    /// </summary>
    [HttpPost("give-to-customers-with-preference")]
    public Task<int> GivePromoCodesToCustomersWithPreference([FromBody] GivePromoCodeRequestDto data)
    {
        return _mediator.Send(new GivePromoCodesToCustomerCommand
        {
            Data = data
        });
    }

    /// <summary>
    /// Получить список промокодов
    /// </summary>
    [HttpGet]
    public Task<IEnumerable<PromoCodeResponseDto>> GetPromoCodes([FromQuery] GetPromoCodeRequestDto data)
    {
        return _mediator.Send(new GetPromoCodesQuery
        {
            Data = data
        });
    }
}