using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Requests;
using PromoCodeFactory.WebHost.Models.Responses;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomerController(ICustomerRepository customerRepository, IMapper mapper) : ControllerBase
        
    {

        //private readonly IRepository<Customer> _customerRepository;
        //private readonly IRepository<Preference> _preferenceRepository;

        //public CustomersController(IRepository<Customer> customerRepository, 
        //    IRepository<Preference> preferenceRepository)
        //{
        //    _customerRepository = customerRepository;
        //    _preferenceRepository = preferenceRepository;
        //}
        /// <summary>
        /// Get the customer data by Id
        /// </summary>
        /// <returns></returns>


        //[HttpGet]
        //public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        //{
        //    var customers =  await _customerRepository.AllAsync;

        //    var response = customers.Select(x => new CustomerShortResponse()
        //    {
        //        Id = x.Id,
        //        Email = x.Email,
        //        FirstName = x.FirstName,
        //        LastName = x.LastName
        //    }).ToList();

        //    return Ok(response);
        //}

        //[HttpGet("{id:guid}")]
        //public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        //{
        //    var customer =  await _customerRepository.GetByIdAsync(id);

        //    var response = new CustomerResponse(customer);

        //    return Ok(response);
        //}

        //[HttpPost]
        //public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        //{
        //    //Получаем предпочтения из бд и сохраняем большой объект
        //    var preferences = await _preferenceRepository
        //        .GetRangeByIdsAsync(request.PreferenceIds);

        //    var customer = new Customer()
        //    {
        //        Email = request.Email,
        //        FirstName = request.FirstName,
        //        LastName = request.LastName,
        //    };
        //    customer.Preferences = preferences.Select(x => new CustomerPreference()
        //    {
        //        Customer = customer,
        //        Preference = x
        //    }).ToList();

        //    await _customerRepository.AddAsync(customer);

        //    return CreatedAtAction(nameof(GetCustomerAsync), new {id = customer.Id}, null);
        //}

        //[HttpPut("{id:guid}")]
        //public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        //{
        //    var customer = await _customerRepository.GetByIdAsync(id);

        //    if (customer == null)
        //        return NotFound();

        //    var preferences = await _preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);

        //    customer.Email = request.Email;
        //    customer.FirstName = request.FirstName;
        //    customer.LastName = request.LastName;
        //    customer.Preferences.Clear();
        //    customer.Preferences = preferences.Select(x => new CustomerPreference()
        //    {
        //        Customer = customer,
        //        Preference = x
        //    }).ToList();

        //    await _customerRepository.UpdateAsync(customer);

        //    return NoContent();
        //}

        //[HttpDelete("{id:guid}")]
        //public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        //{
        //    var customer = await _customerRepository.GetByIdAsync(id);

        //    if (customer == null)
        //        return NotFound();

        //    await _customerRepository.DeleteAsync(customer);

        //    return NoContent();
        //}
        /// <summary>
        /// Get data from all customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), 200)]
        public async Task<IEnumerable<CustomerShortResponse>> GetAll() =>
            (await customerRepository.GetAllAsync()).Select(mapper.Map<CustomerShortResponse>);

        /// <summary>
        /// Get customer data by Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CustomerResponse>> Get(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);
            if (customer == null) return NotFound();
            else return Ok(mapper.Map<CustomerResponse>(customer));
        }

        /// <summary>
        /// Add a new customer with preferences
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync([FromBody] CreateOrEditCustomerRequest request)
        {
            if (request.PreferenceIds.Count() == 0) return BadRequest("To get coupons, you need to have at least one preference");
            else
            {
                var response = await customerRepository.CreateAsync(mapper.Map<Customer>(request));
                return CreatedAtAction(nameof(Get), new { id = response.Id }, mapper.Map<CustomerResponse>(response));
            }
        }
        /// <summary>
        /// Update customer details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> EditCustomersAsync(Guid id, [FromBody] CreateOrEditCustomerRequest request)
        {
            if ((await customerRepository.GetByIdAsync(id)) == null) return NotFound("Employee id not found");
            else if (request.PreferenceIds.Count() == 0) return BadRequest("To get coupons, you need to have at least one preference");
            else
            {
                await customerRepository.UpdateAsync(id, mapper.Map<Customer>(request));
                return NoContent();
            }
        }

        /// <summary>
        /// Delete a customer by Id along with his unique promotional codes 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            if ((await customerRepository.GetByIdAsync(id)) == null) return NotFound("Employee id not found");
            else
            {
                await customerRepository.DeleteAsync(id);
                return NoContent();
            }
        }
    }
}