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

        protected readonly IRepository<Customer> _customerRepository;
        protected readonly IRepository<Preference> _preferenceRepository;
        protected readonly IRepository<CustomerPreference> _customerPreferenceRepository;
        protected readonly IRepository<PromoCode> _promoCodeRepository;

        public CustomersController(
            IRepository<Customer> customerRepository,
            IRepository<CustomerPreference> customerPreferenceRepository,
            IRepository<Preference> preferenceRepository,
            IRepository<PromoCode> promoCodeRepository)
        {
            _customerRepository = customerRepository;
            _customerPreferenceRepository = customerPreferenceRepository;
            _preferenceRepository = preferenceRepository;
            _promoCodeRepository = promoCodeRepository;
        }

        /// <summary>
        /// Получение списка клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();

            var response = customers.Select(c => new CustomerShortResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получение списка клиентов по id вместе с выданными ему промокодами
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                    return NotFound();

                var customerPreferences = (await _customerPreferenceRepository.GetAllAsync())
                    .Where(cp => cp.CustomerId == id)
                    .ToList();


                var preferenceIds = customerPreferences
                    .Select(cp => cp.PreferenceId)
                    .Distinct()
                    .ToList();

                var preferencesDictionary = (await _preferenceRepository.GetAllAsync())
                    .Where(p => preferenceIds.Contains(p.Id))
                    .ToDictionary(p => p.Id);


                var response = new CustomerResponse
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,

                    Preferences = customerPreferences
                        .Where(cp => preferencesDictionary.ContainsKey(cp.PreferenceId))
                        .Select(cp => new PreferenceResponse
                        {
                            Id = cp.PreferenceId,
                            Name = preferencesDictionary[cp.PreferenceId].Name
                        })
                        .ToList(),

                    PromoCodes = customer.Promocodes
                        .Select(p => new PromoCodeShortResponse
                        {
                            Id = p.Id,
                            Code = p.Code,
                            ServiceInfo = p.ServiceInfo,
                            BeginDate = p.BeginDate.ToString("yyyy-MM-dd"),
                            EndDate = p.EndDate.ToString("yyyy-MM-dd"),
                            PartnerName = p.PartnerName
                        })
                        .ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Cоздание нового клиента вместе с его предпочтениями
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            try
            {
                request.PreferenceIds ??= new List<Guid>();

                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                };

                await _customerRepository.AddAsync(customer);

                var preferenceIds = request.PreferenceIds.Distinct().ToList();
                if (preferenceIds.Count > 0)
                {
                    foreach (var prefId in preferenceIds)
                    {
                        await _customerPreferenceRepository.AddAsync(new CustomerPreference
                        {
                            CustomerId = customer.Id,
                            PreferenceId = prefId
                        });
                    }
                }

                return CreatedAtAction(
                    nameof(GetCustomerAsync),
                    new { id = customer.Id },
                    null
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        /// <summary>
        /// Обновление клиента вместе с его предпочтениями
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            try
            {
                request.PreferenceIds ??= new List<Guid>();

                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                    return NotFound();

                existingCustomer.FirstName = request.FirstName;
                existingCustomer.LastName = request.LastName;
                existingCustomer.Email = request.Email;

                
                await _customerRepository.UpdateAsync(existingCustomer);

                var existingPrefs = (await _customerPreferenceRepository.GetAllAsync())
                .Where(cp => cp.CustomerId == id)
                .ToList();

                foreach (var cp in existingPrefs)
                    await _customerPreferenceRepository.DeleteAsync(cp.Id);

                var preferenceIds = request.PreferenceIds.Distinct().ToList();
                if (preferenceIds.Count > 0)
                {
                    foreach (var prefId in preferenceIds)
                    {
                        await _customerPreferenceRepository.AddAsync(new CustomerPreference
                        {
                            CustomerId = id,
                            PreferenceId = prefId
                        });
                    }
                }
                return NoContent();
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

        }

        /// <summary>
        /// Удаление клиента вместе с выданными ему промокодами
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> DeleteCustomer(Guid id)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(id);

                if (existingCustomer == null) return NotFound();

                // remove issued promocodes
                var issuedPromoCodes = (await _promoCodeRepository.GetAllAsync())
                    .Where(pc => pc.CustomerId == id)
                    .ToList();

                foreach (var promoCode in issuedPromoCodes)
                {
                    await _promoCodeRepository.DeleteAsync(promoCode.Id);
                }

                // remove customer preferences
                var customerPreferences = (await _customerPreferenceRepository.GetAllAsync())
                    .Where(cp => cp.CustomerId == id)
                    .ToList();

                foreach (var cp in customerPreferences)
                {
                    await _customerPreferenceRepository.DeleteAsync(cp.Id);
                }

                var result = await _customerRepository.DeleteAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}