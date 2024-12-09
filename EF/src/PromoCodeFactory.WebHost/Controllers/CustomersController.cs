using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<Customer> _customersRepository;

        public CustomersController(IRepository<Customer> customersRepository) => _customersRepository = customersRepository;

        /// <summary>
        /// Получение списка клиентов
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync(CancellationToken cancellationToken = default)
        {
            var customers = await _customersRepository.GetAllAsync(cancellationToken);

            var shortCustomers = customers.Select(x => new CustomerShortResponse
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Id = x.Id,
            });

            return Ok(shortCustomers);
        }

        /// <summary>
        /// Получение клиента вместе с выданными ему промомкодами
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var customer = await _customersRepository.GetByIdAsync(id, cancellationToken);
            if (customer == null)
                return NotFound();

            var response = new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = customer.CustomerPreferences?.Select(c => new PreferenceResponse { Id = c.PreferenceId, Name = c.Preference.Name }).ToList(),
                PromoCodes = customer.PromoCodes?.Select(p => new PromoCodeShortResponse
                {
                    BeginDate = p.BeginDate.ToShortDateString(),
                    EndDate = p.EndDate.ToShortDateString(),
                    Code = p.Code,
                    PartnerName = p.PartnerName,
                    ServiceInfo = p.ServiceInfo,
                    Id = p.Id
                }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Создание нового клиента вместе с его предпочтениями
        /// </summary>
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request, CancellationToken cancellationToken = default)
        {
            var customerId = Guid.NewGuid();
            var customer = new Customer()
            {
                Id = customerId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomerPreferences = request.PreferenceIds.Select(id => new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    PreferenceId = id
                }).ToList()
            };
        
            await _customersRepository.CreateAsync(customer, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Обновить данные клиента вместе с его предпочтениями
        /// </summary>
        [HttpPut("{customerId}")]
        public async Task<IActionResult> EditCustomersAsync(Guid customerId, CreateOrEditCustomerRequest request, CancellationToken cancellationToken)
        {
            var customer = await _customersRepository.GetByIdAsync(customerId, cancellationToken);

            if (customer == null)
                return NotFound();

            customer = new Customer()
            {
                Id = customerId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomerPreferences = request.PreferenceIds.Select(id => new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    PreferenceId = id
                }).ToList()
            };

            await _customersRepository.UpdateAsync(customer,cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Удаление клиента вместе с выданными ему промокодами
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
        {
            var customer = await _customersRepository.GetByIdAsync(id,cancellationToken);

            if (customer == null)
                return NotFound();

            await _customersRepository.DeleteAsync(id, cancellationToken);
            return Ok();
        }
    }
}