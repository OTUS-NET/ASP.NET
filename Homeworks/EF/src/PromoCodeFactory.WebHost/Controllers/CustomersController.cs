using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController(IRepository<Customer> customerRepository)
        : ControllerBase
    {
        /// <summary>
        /// Получить список клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync()
        {
            var result = await customerRepository.GetAllAsync();

            var customerShortResponse = result.Select( x => new CustomerShortResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            });

            return Ok(customerShortResponse);
        }

        /// <summary>
        /// Получение клиента вместе с выданными ему промомкодами
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var promoCodesShortResponse = (customer.PromoCodes != null) ?
                customer.PromoCodes.Select(p => new PromoCodeShortResponse
                {
                    Id = p.Id,
                    Code = p.Code,
                    ServiceInfo = p.ServiceInfo,
                    BeginDate = p.BeginDate.ToString(),
                    EndDate = p.EndDate.ToString(),
                    PartnerName = p.PartnerName
                }).ToList() :
                null;

            var preferencesResponse = customer.CustomerPreferences.Select( cp => new PreferenceResponse
            {
                Id = cp.PreferenceId,
                Name = cp.Preference.Name
            });

            var customerResponse = new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PromoCodes = promoCodesShortResponse,
                Preferences = preferencesResponse.ToList()
            };

            return Ok(customerResponse);
        }

        /// <summary>
        /// Создание нового клиента вместе с его предпочтениями
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customerId = Guid.NewGuid();
            var customer = new Customer
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

            await customerRepository.AddAsync(customer);

            return Ok(customer.Id);
        }

        /// <summary>
        /// Обновить данные клиента вместе с его предпочтениями
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            customer = new Customer()
            {
                Id = customer.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomerPreferences = request.PreferenceIds.Select(id => new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    PreferenceId = id
                }).ToList()
            };

            await customerRepository.UpdateAsync(customer);

            return Ok();
        }

        /// <summary>
        /// Удаление клиента вместе с выданными ему промокодами
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            await customerRepository.DeleteAsync(id);
            
            return Ok();
        }
    }
}