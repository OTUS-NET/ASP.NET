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
    public class CustomersController: ControllerBase
    {
        private readonly ICustomerRepository _repository;
        private readonly IRepository<Preference> _prefRepository;

        public CustomersController(ICustomerRepository repository, IRepository<Preference> prefRepository)
        {
            _repository = repository;
            _prefRepository = prefRepository;
        }
        /// <summary>
        /// Получения всех Customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await _repository.GetAllAsync();

            var customerShortResponseList = customers.Select(x =>
                new CustomerShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }).ToList();

            return customerShortResponseList;
        }

        /// <summary>
        /// Получение одного Customer по id с дополнительными данными по Preference и PromoCode
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _repository.GetCustomerWithAllProperties(id);

            if (customer == null)
                return NotFound();

            var customerResponse = new CustomerResponse()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = customer.CustomerPreferences.Select(x =>
                new PreferenceResponse()
                {
                    Id = x.PreferenceId,
                    Name = x.Preference.Name
                }
                ).ToList(),
                PromoCodes = customer.PromoCodes.Select(x => new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    ServiceInfo = x.ServiceInfo,
                    BeginDate = x.BeginDate.ToLongDateString(),
                    EndDate = x.EndDate.ToLongDateString(),
                    Code = x.Code,
                    PartnerName = x.PartnerName

                }).ToList()
            };
            return customerResponse;
        }

        /// <summary>
        /// Добавление нового Customer
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync([FromBody]CreateOrEditCustomerRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName))
                return BadRequest("Поля Email и Name обязательны");

            var existingEmployee = (await _repository.GetAllAsync())
                .FirstOrDefault(e => e.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

            if (existingEmployee != null)
                return Conflict("Сотрудник с таким email уже существует");

            var preferenceDB = await _prefRepository.GetAllAsync();
            var preferences = preferenceDB.Where(x => request.PreferenceIds.Any(p => p == x.Id)).ToList();

            if (preferences is null || !preferences.Any())
                return BadRequest("Не существует выбранного Preference.");

            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomerPreferences = preferences.Select(x => 
                new CustomerPreference() 
                { 
                    PreferenceId = x.Id
                }).ToList()
            };

            var addedCustomer = await _repository.AddAsync(customer);

            // response
            var response = new CustomerResponse
            {
                Id = addedCustomer.Id,
                Email = addedCustomer.Email,
                FirstName = addedCustomer.FullName,
                LastName = addedCustomer.LastName,
                Preferences = addedCustomer.CustomerPreferences.Select(x =>
                new PreferenceResponse()
                {
                    Id = x.PreferenceId,
                    Name = x.Preference.Name
                }
                ).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Изменение данных о Customer
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            //TODO: Обновить данные клиента вместе с его предпочтениями
            var customerDb = await _repository.GetByIdAsync(id);
            if (customerDb == null)
                return NotFound();

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName))
                return BadRequest("Поля Email и Name обязательны");

            var preferenceDB = await _prefRepository.GetAllAsync();
            var preferences = preferenceDB.Where(x => request.PreferenceIds.Any(p => p == x.Id)).ToList();

            if (preferences is null || !preferences.Any())
                return BadRequest("Не существует выбранного Preference.");

            customerDb.FirstName = request.FirstName;
            customerDb.LastName = request.LastName;
            customerDb.Email = request.Email;
            customerDb.CustomerPreferences = preferences.Select(x => 
            new CustomerPreference()
            { 
                PreferenceId = x.Id, 
                CustomerId = customerDb.Id 
            }).ToList();

            var updatedCustomer = await _repository.UpdateCustomerAsync(customerDb);

            // response
            var response = new CustomerResponse
            {
                Id = updatedCustomer.Id,
                Email = updatedCustomer.Email,
                FirstName = updatedCustomer.FullName,
                LastName = updatedCustomer.LastName,
                Preferences = updatedCustomer.CustomerPreferences.Select(x =>
                new PreferenceResponse()
                { 
                    Id = x.PreferenceId,
                    Name = x.Preference.Name}
                ).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Удаление Customer
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customerDb = await _repository.GetByIdAsync(id);
            if (customerDb == null)
                return NotFound();

            var result = await _repository.RemoveAsync(id);

            if (!result)
                return StatusCode(500, "Не удалось удалить Customer");

            return NoContent();
        }
    }
}