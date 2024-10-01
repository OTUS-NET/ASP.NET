//using Mapster;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]/[Action]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _repo;

        public CustomersController(IRepository<Customer> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Получить всех Клиентов.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomers(CancellationToken cts)
        {
            var customers = await _repo.GetAllAsync(cts);
            return Ok(customers.Count() > 0 ? customers.ToResponseList() : default);
        }

        /// <summary>
        /// Получить Клиента по id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomer(Guid id, CancellationToken cts)
        {
            var customer = await _repo.GetByIdAsync(id, cts);
            if (customer == null) { return BadRequest($"No {nameof(Customer)} was found"); }
            return Ok(customer.ToResponse());
        }

        /// <summary>
        /// Создать Клиента.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateOrEditCustomerRequest request, CancellationToken cts)
        {
            var newCustomer = request.ToCustomer();
            await _repo.CreateAsync(newCustomer, cts);
            return CreatedAtAction(nameof(GetCustomer), new { id = newCustomer.Id }, newCustomer.ToResponse());
        }

        /// <summary>
        /// Редактировать Клиента.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomer(Guid id, CreateOrEditCustomerRequest request, CancellationToken cts)
        {
            var updateCustomer = request.ToCustomer(id);
            await _repo.UpdateAsync(id, updateCustomer, cts);
            return NoContent();
        }

        /// <summary>
        /// Удалить Клиента.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cts)
        {
            await _repo.DeleteByIdAsync(id, cts);
            return Ok();
        }
    }
}