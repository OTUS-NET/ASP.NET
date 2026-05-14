using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

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

        public CustomersController(
            IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Получить список всех клиентов
        /// </summary>
        [HttpGet]
        public async Task<List<CustomerShortResponse>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();

            return customers.Select(x => new CustomerShortResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            }).ToList();
        }

        /// <summary>
        /// Получить клиента по Id
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            return Ok(new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = customer.CustomerPreferences
                    .Select(x => new PreferenceResponse
                    {
                        Id = x.Preference.Id,
                        Name = x.Preference.Name
                    }).ToList(),
                PromoCodes = customer.PromoCodes
                    .Select(x => new PromoCodeShortResponse
                    {
                        Id = x.Id,
                        Code = x.Code,
                        ServiceInfo = x.ServiceInfo,
                        BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                        EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                        PartnerName = x.PartnerName
                    }).ToList()
            });
        }

        /// <summary>
        /// Создать клиента
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync([FromBody] CreateOrEditCustomerRequest request)
        {
            var preferences = await _preferenceRepository.GetAllAsync();
            var selectedPreferences = preferences
                .Where(x => request.PreferenceIds.Contains(x.Id))
                .ToList();

            var customerId = Guid.NewGuid();

            var customer = new Customer
            {
                Id = customerId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomerPreferences = selectedPreferences
                    .Select(x => new CustomerPreference
                    {
                        CustomerId = customerId,
                        PreferenceId = x.Id
                    }).ToList()
            };

            var createdCustomer = await _customerRepository.CreateAsync(customer);
            var customerFromDb = await _customerRepository.GetByIdAsync(createdCustomer.Id);

            return CreatedAtAction(nameof(GetCustomerAsync), new { id = createdCustomer.Id }, new CustomerResponse
            {
                Id = customerFromDb.Id,
                FirstName = customerFromDb.FirstName,
                LastName = customerFromDb.LastName,
                Email = customerFromDb.Email,
                Preferences = customerFromDb.CustomerPreferences
                    .Select(x => new PreferenceResponse
                    {
                        Id = x.Preference.Id,
                        Name = x.Preference.Name
                    }).ToList(),
                PromoCodes = customerFromDb.PromoCodes
                    .Select(x => new PromoCodeShortResponse
                    {
                        Id = x.Id,
                        Code = x.Code,
                        ServiceInfo = x.ServiceInfo,
                        BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                        EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                        PartnerName = x.PartnerName
                    }).ToList()
            });
        }

        /// <summary>
        /// Обновить клиента
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> EditCustomersAsync(Guid id, [FromBody] CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            var preferences = await _preferenceRepository.GetAllAsync();
            var selectedPreferences = preferences
                .Where(x => request.PreferenceIds.Contains(x.Id))
                .ToList();

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;

            customer.CustomerPreferences.Clear();

            foreach (var preference in selectedPreferences)
            {
                customer.CustomerPreferences.Add(new CustomerPreference
                {
                    CustomerId = customer.Id,
                    PreferenceId = preference.Id
                });
            }

            await _customerRepository.UpdateAsync(customer);

            return NoContent();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            await _customerRepository.DeleteAsync(customer);

            return NoContent();
        }
    }
}