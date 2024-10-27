using MediatR;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Commands.Commands.Customers;
using PromoCodeFactory.Commands.Queries.Customers;
using PromoCodeFactory.Contracts;
using PromoCodeFactory.Contracts.Customers;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Контроллер для управления клиентами
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Получить список всех клиентов
    /// </summary>
    [HttpGet]
    public Task<IEnumerable<CustomerResponseDto>> GetAllCustomers()
    {
        return _mediator.Send(new GetAllCustomersQuery());
    }

    /// <summary>
    /// Получить клиента по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public Task<CustomerResponseDto> GetCustomerById([FromRoute] Guid id)
    {
        return _mediator.Send(new GetCustomerByIdQuery
        {
            Id = id,
        });
    }

    /// <summary>
    /// Создать нового клиента
    /// </summary>
    [HttpPost]
    public Task<ResponseId<Guid>> CreateCustomer([FromBody] CustomerSetDto data)
    {
        return _mediator.Send(new CreateCustomerCommand
        {
            Data = data,
        });
    }

    /// <summary>
    /// Обновить данные клиента
    /// </summary>
    [HttpPut("{id:guid}")]
    public Task UpdateCustomer([FromRoute] Guid id, [FromBody] CustomerSetDto data)
    {
        return _mediator.Send(new UpdateCustomerCommand
        {
            Id = id,
            Data = data
        });
    }

    /// <summary>
    /// Удалить клиента
    /// </summary>
    [HttpDelete("{id:guid}")]
    public Task DeleteCustomer([FromRoute] Guid id)
    {
        return _mediator.Send(new DeleteCustomerCommand
        {
            Id = id,
        });
    }
}