using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

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
        private readonly IMapper _mapper;

        public CustomersController(IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить данные всех клиентов
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            var customers = (await _customerRepository.GetAllAsync(cancellationToken, true))
                              .Select(c => _mapper.Map<CustomerShortResponse>(c)).ToList(); ;
            return Ok(customers);
        }

        /// <summary>
        /// Получить данные клиента по id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetAsync(id, cancellationToken);
            if (customer is null)
                return NotFound($"No Customer with Id {id} found");

            var customerResponse = _mapper.Map<CustomerResponse>(customer);
            return Ok(customerResponse);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request, CancellationToken cancellationToken)
        {
            var customer = _mapper.Map<Customer>(request);
            if (customer.Preferences == null)
                customer.Preferences = new List<Preference>();
            foreach (var preferenceId in request.PreferenceIds)
            {
                var preference = await _preferenceRepository.GetAsync(preferenceId, cancellationToken);
                if (preference != null)
                {
                    customer.Preferences.Add(preference);
                }
            }
            await _customerRepository.AddAsync(customer, cancellationToken);

            return Created();
        }

        /// <summary>
        /// Отредактировать клиента с id
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request, CancellationToken token)
        {
            var customer = await _customerRepository.GetAsync(id, token);

            if (customer == null)
                return NotFound();

            customer.Email = request.Email;
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            if (customer.Preferences == null)
                customer.Preferences = new List<Preference>();
            customer.Preferences.Clear();
            foreach (var preferenceId in request.PreferenceIds)
            {
                var preference = await _preferenceRepository.GetAsync(preferenceId, token);
                if (preference != null)
                {
                    customer.Preferences.Add(preference);
                }
            }

            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync(token);
            return NoContent();
        }

        /// <summary>
        /// Удалить клиента с id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetAsync(id, cancellationToken);

            if (customer == null)
                return NotFound();

            await _customerRepository.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}