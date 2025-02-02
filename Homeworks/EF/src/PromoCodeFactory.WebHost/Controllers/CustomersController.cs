using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Extensions;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
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
        public CustomersController(
            IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository,
            IRepository<PromoCode> promoCodeRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
            _promoCodeRepository = promoCodeRepository;
        }

        /// <summary>
        /// Получить список клиентов
        /// </summary>
        /// <returns>Список клиентов</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            var response = customers.Select(c => c.ToShortResponse()).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Получить клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Информация о клиенте</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            var response = customer.ToResponse();
            return Ok(response);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <param name="request">Данные для создания клиента</param>
        /// <returns>Результат операции</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var preferences = await _preferenceRepository.GetAllAsync();
            var customer = request.ToCustomer(preferences);
            await _customerRepository.AddAsync(customer);
            return Ok();
        }

        /// <summary>
        /// Обновить данные клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="request">Данные для обновления</param>
        /// <returns>Результат операции</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            var preferences = await _preferenceRepository.GetAllAsync();
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.Preferences = preferences.Where(p => request.PreferenceIds.Contains(p.Id)).ToList();

            await _customerRepository.UpdateAsync(customer);
            return Ok();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            foreach (var promoCode in customer.PromoCodes)
            {
                await _promoCodeRepository.DeleteAsync(promoCode.Id);
            }

            await _customerRepository.DeleteAsync(id);
            return Ok();
        }
    }
}