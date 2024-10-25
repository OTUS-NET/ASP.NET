using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly EfRepository<Customer> _customerRepository;

        public CustomersController(EfRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync(HttpContext.RequestAborted);
                var response = customers.Select(c => new CustomerShortResponse
                {
                    Id = c.Id,
                    Email = c.Email,
                    FirstName = c.FirstName,
                    LastName = c.LastName
                }).ToList();

                return Ok(response);
            }
            catch(OperationCanceledException)
            {
                return StatusCode(499, "Client Closed Request");
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(id, HttpContext.RequestAborted);
                if (customer == null)
                {
                    return NotFound();
                }

                var response = new CustomerResponse
                {
                    Id = customer.Id,
                    Email = customer.Email,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    CustomerPreferences = customer.CustomerPreferences.Select(pr =>pr.Preference).Select(n => n.Name).ToList()
                };

                return Ok(response);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Client Closed Request");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            try
            {
                var customer = new Customer
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CustomerPreferences = new List<CustomerPreference>(),

                };
                foreach (var preference in request.Preferences)
                {
                    var custPref = new CustomerPreference()
                    {
                        Customer = customer,
                        Preference = preference
                    };
                    customer.CustomerPreferences.Add(custPref);
                }
                await _customerRepository.CreateAsync(customer, HttpContext.RequestAborted);

                return CreatedAtAction(nameof(GetCustomerAsync), new { id = customer.Id }, customer);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Client Closed Request");
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomerAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id, HttpContext.RequestAborted);
                if (customer == null)
                {
                    return NotFound();
                }

                customer.Email = request.Email;
                customer.FirstName = request.FirstName;
                customer.LastName = request.LastName;


                await _customerRepository.UpdateAsync(customer, HttpContext.RequestAborted);

                return NoContent();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Client Closed Request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id, HttpContext.RequestAborted);
                if (customer == null)
                {
                    return NotFound();
                }

                await _customerRepository.DeleteAsync(id, HttpContext.RequestAborted);

                return NoContent();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Client Closed Request");
            }
        }
    }
}