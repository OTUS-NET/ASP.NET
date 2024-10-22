using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Common.Extensions;
using PromoCodeFactory.Contracts.Customers;
using PromoCodeFactory.Core.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Контроллер для управления клиентами
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController(IRepository<Customer> customerRepository, IRepository<PromoCode> promoCodeRepository)
    : ControllerBase
{
    private readonly IRepository<Customer> _customerRepository = customerRepository;
    private readonly IRepository<PromoCode> _promoCodeRepository = promoCodeRepository;

    /// <summary>
    /// Получить список всех клиентов
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CustomerResponseDto>>> GetCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        
        var customerResponses = customers.Select(x => x.MapToCustomerResponseDto()).ToList();
        
        return Ok(customerResponses);
    }

    /// <summary>
    /// Получить клиента по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponseDto>> GetCustomerByIdAsync([FromRoute] Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer.MapToCustomerResponseDto());
    }

    /// <summary>
    /// Создать нового клиента
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateCustomerAsync([FromBody] CustomerSetDto customerDto)
    {
        var customer = new Customer
        {
            FirstName = customerDto.FirstName,
            LastName = customerDto.LastName,
            Email = customerDto.Email
        };

        await _customerRepository.CreateAsync(customer);

        return Ok();
    }

    /// <summary>
    /// Обновить данные клиента
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateCustomerAsync([FromRoute] Guid id, [FromBody] CustomerSetDto customerDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        customer.FirstName = customerDto.FirstName;
        customer.LastName = customerDto.LastName;
        customer.Email = customerDto.Email;

        await _customerRepository.UpdateAsync(customer);
        
        return NoContent();
    }

    /// <summary>
    /// Удалить клиента
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCustomerAsync([FromRoute] Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        // Удаление связанных промокодов
        var promoCodes = await _promoCodeRepository.GetAllAsync();
        
        var customerPromoCodes = promoCodes.Where(p => p.PartnerManagerId == id).ToList();
        foreach (var promoCode in customerPromoCodes)
        {
            await _promoCodeRepository.DeleteAsync(promoCode.Id);
        }

        await _customerRepository.DeleteAsync(id);
        return NoContent();
    }
}