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
    //TODO:
    //Promocodes controller ( 7)
    //Удалить промокоды при удалении клиента
    //GivePromocodesToCustomersWithPreferenceAsync
    //должен сохранять новый промокод в базе данных и находить клиентов с переданным предпочтением и добавлять им данный промокод.GetPromocodesAsync - здесь даты передаются строками, чтобы не было проблем с часовыми поясами

    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {

        private IRepository<Customer> _customerRepository;
        private IRepository<Preference> _preferenceRepository;
        public CustomersController
            (
            IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository
            ) 
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Получить список кратких описаний всех клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            var dbResult = await _customerRepository.GetAllAsync();

            var result = dbResult.Select(x => new CustomerShortResponse()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Id = x.Id
            });

            return Ok(result);
        }

        /// <summary>
        /// Получить клиента по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType<CustomerResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            //Добавить получение клиента вместе с выданными ему промомкодами
            var dbResult = await _customerRepository.GetByIdAsync(id);

            if (dbResult == null)
                return NotFound();

            //Тут только конверсия
            var response = new CustomerResponse()
            {
                FirstName = dbResult.FirstName,
                LastName = dbResult.LastName,
                Email = dbResult.Email,
                Id = dbResult.Id
            };
            response.PromoCodes = new List<PromoCodeShortResponse>(
                dbResult.Promocodes.Select(
                    x => new PromoCodeShortResponse() 
                    {
                        Code = x.Code,
                        Id = x.Id,
                        ServiceInfo = x.ServiceInfo,
                        BeginDate = x.BeginDate.ToString(),
                        EndDate = x.EndDate.ToString(),
                        PartnerName = x.PartnerName
                    }));

            response.Preferences = new List<PreferenceResponse>(
                dbResult.Preferences.Select(
                    x => new PreferenceResponse()
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }));

            return Ok(response);

        }

        /// <summary>
        /// Создать клиента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType<CustomerResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            if (!ValidateCustomerRequest(request))
                return BadRequest();

            //Добавить создание нового клиента вместе с его предпочтениями
            var newCustomer = new Customer()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Preferences = new List<Preference>
                (
                    //TODO: валидация существования преференсов
                    await _preferenceRepository.GetAllAsync(x => request.PreferenceIds.Contains(x.Id))
                )
            };

            //Создание
            var dbResult = await _customerRepository.CreateAsync(newCustomer);
            if(dbResult == null) return BadRequest();

            //Тут только конверсия
            var response = new CustomerResponse()
            {
                FirstName = dbResult.FirstName,
                LastName = dbResult.LastName,
                Email = dbResult.Email,
                Id = dbResult.Id
            };
            response.PromoCodes = new List<PromoCodeShortResponse>(
                dbResult.Promocodes.Select(
                    x => new PromoCodeShortResponse()
                    {
                        Code = x.Code,
                        Id = x.Id,
                        ServiceInfo = x.ServiceInfo,
                        BeginDate = x.BeginDate.ToString(),
                        EndDate = x.EndDate.ToString(),
                        PartnerName = x.PartnerName
                    }));
            response.Preferences = new List<PreferenceResponse>(
                dbResult.Preferences.Select(
                    x => new PreferenceResponse()
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }));

            return Created("api/v1/Customers", response);
        }

        /// <summary>
        /// Редактировать клиента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            if (!ValidateCustomerRequest(request))
                return BadRequest();

            //Добавить создание нового клиента вместе с его предпочтениями
            var customer = new Customer()
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Preferences = new List<Preference>
                (
                    //TODO: валидация существования преференсов
                    await _preferenceRepository.GetAllAsync(x => request.PreferenceIds.Contains(x.Id))
                )
            };

            var dbResult = await _customerRepository.UpdateAsync(customer);
            return dbResult != null ? NoContent() : BadRequest();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            var dbResult = await _customerRepository.DeleteAsync(id);
            return dbResult == Guid.Empty ? BadRequest() : NoContent();
        }

        /// <summary>
        /// Валидация запроса на создание клиента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool ValidateCustomerRequest(CreateOrEditCustomerRequest request)
        {
            //TODO
            //Заглушка
            return true;
        }
    }
}