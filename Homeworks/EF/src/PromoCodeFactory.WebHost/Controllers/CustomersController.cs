using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController(
        ILogger<CustomersController> logger,
        IRepository<Customer> customersRepository,
        IRepository<Preference> preferenceRepository
    ) : ControllerBase
    {
        /// <summary>
        /// Получение списка клиентов
        /// </summary>
        /// <remarks>
        /// Возвращает полный список клиентов, зарегистрированных в БД 
        /// </remarks>
        /// <returns>Список из <see cref="CustomerShortResponse" /></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            try
            {
                var customers = await customersRepository.GetAllAsync();
                return Ok(customers.Select(x => new CustomerShortResponse
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }).ToList());
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get list of customers: [{msg}]", e.Message);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Данные о клиенте
        /// </summary>
        /// <remarks>
        /// Возвращает полные данные
        /// </remarks>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Полные данные пользователя <see cref="CustomerResponse"/></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            try
            {
                var customer = await customersRepository.GetByIdAsync(id);
                if (customer == null)
                    return NotFound();

                return Ok(CreateCustomerResponse(customer));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error get customer with id [{id:D}]: {msg}", id, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Создание нового клиента
        /// </summary>
        /// <param name="request">Данные создаваемого клиента <see cref="CreateOrEditCustomerRequest"/></param>
        /// <returns>Код 201 при успешном создании клиента, 500 в случае ошибки</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            try
            {
                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Preferences = await GetPreferences(request.PreferenceIds),
                    PromoCodes = []
                };

                await customersRepository.AddAsync(customer);
                return Created(
                    HttpContext.Request.Path.Add(new PathString($"/{customer.Id:D}")),
                    CreateCustomerResponse(customer)
                );
            }
            catch (KeyNotFoundException e)
            {
                logger.LogError(e, "Bad request on customer creation: {msg}", e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error on customer creation: {msg}", e.Message);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Обновление данных клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="request">Новые данные пользователя</param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            try
            {
                var customer = await customersRepository.GetByIdAsync(id);
                if (customer == null)
                    return NotFound(id);

                customer.Email = request.Email;
                customer.FirstName = request.FirstName;
                customer.LastName = request.LastName;

                customer.Preferences = await GetPreferences(request.PreferenceIds);

                await customersRepository.UpdateAsync(customer);
                return Ok(CreateCustomerResponse(customer));
            }
            catch (KeyNotFoundException e)
            {
                logger.LogError(e, "Bad request on customer update: {msg}", e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error on customer update: {msg}", e.Message);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Удаление клиента по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>200 при успешном удалении, 500 при ошибке, 404 если клиент не найден</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                var customer = await customersRepository.GetByIdAsync(id);
                if (customer == null)
                    return NotFound(id);

                await customersRepository.RemoveAsync(customer);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error on customer deletion: {msg}", e.Message);
                return StatusCode(500, e.Message);
            }
        }

        private async Task<List<Preference>> GetPreferences(IEnumerable<Guid> preferenceIds)
        {
            var preferences = new List<Preference>();
            foreach (var id in preferenceIds)
            {
                var preference = await preferenceRepository.GetByIdAsync(id);
                if (preference == null)
                    throw new KeyNotFoundException($"Preference with id [{id}] was not found");
                preferences.Add(preference);
            }

            return preferences;
        }

        private static CustomerResponse CreateCustomerResponse(Customer customerEntity)
        {
            return new CustomerResponse
            {
                Id = customerEntity.Id,
                Email = customerEntity.Email,
                FirstName = customerEntity.FirstName,
                LastName = customerEntity.LastName,
                PromoCodes = customerEntity.PromoCodes.Select(x => new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    BeginDate = x.BeginDate.ToString(CultureInfo.InvariantCulture),
                    EndDate = x.EndDate.ToString(CultureInfo.InvariantCulture),
                    Code = x.Code,
                    PartnerName = x.PartnerName,
                    ServiceInfo = x.ServiceInfo
                }).ToList(),

                Preferences = customerEntity.Preferences.Select(x => x.Id).ToList()
            };
        }
    }
}