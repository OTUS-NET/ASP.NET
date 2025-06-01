using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
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

        public CustomersController(IRepository<Customer> customerRepository, IRepository<Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }
        /// <summary>
        /// Получить список всех клиентов
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CustomerShortResponse>))]
        public async Task<IActionResult> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();

            var customersModels = customers.Select(c =>
                new CustomerShortResponse()
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email
                });

            return Ok(customersModels);
        }

        /// <summary>
        /// Получить данные клиента по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> GetCustomerAsync(Guid id)
        {
            Customer customer = await _customerRepository.GetByIdAsync(id, c => c.Preferences, c => c.PromoCodes );

            if (customer is null)
                return NotFound();

            var preferences = await _preferenceRepository.GetRangeByIdsAsync(customer.Preferences.Select(p => p.PreferenceId).ToList());

            var customerModel = new CustomerResponse()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = preferences.Select(p => new PreferenceResponse()
                {
                    Id = p.Id,
                    Name = p.Name
                }),
                PromoCodes = customer.PromoCodes.Select(pc => new PromoCodeShortResponse()
                {
                    Id = pc.Id,
                    Code = pc.Code,
                    ServiceInfo = pc.ServiceInfo,
                    BeginDate = pc.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = pc.EndDate.ToString("yyyy-MM-dd"),
                    PartnerName = pc.PartnerName
                })
            };

            return Ok(customerModel);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CustomerShortResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customer = new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };
            var preferences = await _preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);

            customer.Preferences = preferences.Select(p => new CustomerPreference()
            {
                Customer = customer,
                Preference = p
            }).ToList();

            var createdCustomer = await _customerRepository.AddAsync(customer);
            if (createdCustomer is null)
                return BadRequest("Произошла ошибка при создании клиента.");
            var createdCustomerShortResponse = new CustomerShortResponse()
            {
                Id = createdCustomer.Id,
                FirstName = createdCustomer.FirstName,
                LastName = createdCustomer.LastName,
                Email = createdCustomer.Email
            };
            return Created(string.Empty, createdCustomerShortResponse);
        }

        /// <summary>
        /// Редактировать клиента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer is null)
                return NotFound("Клиент с данным id не найден");

            var preferences = await _preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.Preferences = preferences.Select(p => new CustomerPreference()
            {
                Customer = customer,
                Preference = p
            }).ToList();

            await _customerRepository.UpdateAsync(customer);
            return NoContent();
        }

        /// <summary>
        /// Удалить клиента по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            Customer customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null)
                return NotFound("Клиент с данным id не найден");

            await _customerRepository.DeleteAsync(customer);
            return Ok();
        }
    }
}