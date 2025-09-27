using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        IRepository<Customer> customersRepository
        )
        : ControllerBase
    {
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await customersRepository.GetAllAsync();
            
            return Ok(customers.Select(x => new CustomerShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            //TODO: Добавить получение клиента вместе с выданными ему промокодами
            var customer = await customersRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return new CustomerResponse()
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PromoCodes = customer.PromoCodes.Select(x => new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    BeginDate = x.BeginDate.ToString(CultureInfo.InvariantCulture),
                    EndDate = x.EndDate.ToString(CultureInfo.InvariantCulture),
                    Code = x.Code,
                    PartnerName = x.PartnerName,
                    ServiceInfo = x.ServiceInfo
                }).ToList() //await promoCodesRepository.GetByIdAsync(customer.Id).
                
                //customer.
            };
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            //TODO: Добавить создание нового клиента вместе с его предпочтениями
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Preferences = [],
                PromoCodes = []
                //PromoCodes = new List<PromoCodeShortResponse>()
            };

            await customersRepository.AddAsync(customer);
            return Created();

        }

        [HttpPut("{id}")]
        public Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            //TODO: Обновить данные клиента вместе с его предпочтениями
            throw new NotImplementedException();
        }

        [HttpDelete]
        public Task<IActionResult> DeleteCustomer(Guid id)
        {
            //TODO: Удаление клиента вместе с выданными ему промокодами
            throw new NotImplementedException();
        }
    }
}