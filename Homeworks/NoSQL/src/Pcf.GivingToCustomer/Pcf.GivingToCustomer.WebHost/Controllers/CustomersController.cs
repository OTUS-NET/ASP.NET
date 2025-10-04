using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IPreferencesDirectoryGateway _preferencesDirectoryGateway;

        public CustomersController(IRepository<Customer> customerRepository,
            IPreferencesDirectoryGateway preferencesDirectoryGateway)
        {
            _customerRepository = customerRepository;
            _preferencesDirectoryGateway = preferencesDirectoryGateway;
        }
        
        /// <summary>
        /// Получить список клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers =  await _customerRepository.GetAllAsync();

            var response = customers.Select(x => new CustomerShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();

            return Ok(response);
        }
        
        /// <summary>
        /// Получить клиента по id
        /// </summary>
        /// <param name="id">Id клиента, например <example>a6c8c6b1-4349-45b0-ab31-244740aaf0f0</example></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer =  await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            // Получаем имена предпочтений по ID через шлюз
            var preferenceIds = customer.Preferences.Select(p => p.PreferenceId).ToList();
            var preferenceResponses = await Task.WhenAll(
                preferenceIds.Select(id => _preferencesDirectoryGateway.GetPreferenceByIdAsync(id))
            );
            var preferenceDict = preferenceResponses
                .Where(p => p != null)
                .ToDictionary(p => p.Id, p => p.Name);

            var response = new CustomerResponse(customer, preferenceDict);

            return Ok(response);
        }
        
        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var preferenceTasks = request.PreferenceIds.Select(id =>
                _preferencesDirectoryGateway.GetPreferenceByIdAsync(id));
            var preferenceResponses = await Task.WhenAll(preferenceTasks);

            if (preferenceResponses.Any(p => p == null))
                return BadRequest("Одно или несколько предпочтений не найдено");

            Customer customer = CustomerMapper.MapFromModel(request, request.PreferenceIds);
            
            await _customerRepository.AddAsync(customer);

            return CreatedAtAction(nameof(GetCustomerAsync), new {id = customer.Id}, customer.Id);
        }
        
        /// <summary>
        /// Обновить клиента
        /// </summary>
        /// <param name="id">Id клиента, например <example>a6c8c6b1-4349-45b0-ab31-244740aaf0f0</example></param>
        /// <param name="request">Данные запроса></param>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            
            if (customer == null)
                return NotFound();

            var preferenceTasks = request.PreferenceIds.Select(id =>
                _preferencesDirectoryGateway.GetPreferenceByIdAsync(id));
            var preferenceResponses = await Task.WhenAll(preferenceTasks);

            if (preferenceResponses.Any(p => p == null))
                return BadRequest("Одно или несколько предпочтений не найдено");

            CustomerMapper.MapFromModel(request, request.PreferenceIds, customer);

            await _customerRepository.UpdateAsync(customer);

            return NoContent();
        }
        
        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id">Id клиента, например <example>a6c8c6b1-4349-45b0-ab31-244740aaf0f0</example></param>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            
            if (customer == null)
                return NotFound();

            await _customerRepository.DeleteAsync(customer);

            return NoContent();
        }
    }
}