using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess;
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
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly ICustomerPreferenceRepository _customerPreferenceRepository;
        private readonly IPromoCodeCustomersRepository _promotionCodeCustomersRepository;

        public CustomersController(IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository,
            DataContext dataContext,
            ICustomerPreferenceRepository customerPreferenceRepository,
            IPromoCodeCustomersRepository promotionCodeCustomersRepository,
            IRepository<PromoCode> promoCodeRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
            _customerPreferenceRepository = customerPreferenceRepository;
            _promotionCodeCustomersRepository = promotionCodeCustomersRepository;
            _promoCodeRepository = promoCodeRepository;
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
            if (customer is null)
                return NotFound();

            var customerPreferences = await _customerPreferenceRepository.GetAllCustomerPreferencesByCustomerIdAsync(customer.Id);
            var customerPreferencesIds = customerPreferences.Select(x => x.PreferenceId);
            await _preferenceRepository.GetWhere(x => customerPreferencesIds.Contains(x.Id));

            var promocodesCustomer = await _promotionCodeCustomersRepository.GetAllCustomerPromoCodesByCustomerIdAsync(customer.Id);
            var customerPromocodesIds = promocodesCustomer.Select(x => x.PromoCodeId);
            await _promoCodeRepository.GetWhere(x => customerPromocodesIds.Contains(x.Id));

            var response = new CustomerResponse(customer);

            return Ok(response);
        }
        
        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            //Получаем предпочтения из бд и сохраняем большой объект
            var preferences = await _preferenceRepository
                .GetRangeByIdsAsync(request.PreferenceIds);

            Customer customer = CustomerMapper.MapFromModel(request, preferences);
            
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
            
            var preferences = await _preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);
            
            CustomerMapper.MapFromModel(request, preferences, customer);

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