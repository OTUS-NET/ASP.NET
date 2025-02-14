using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Services.Abstractions;
using PromoCodeFactory.Services.Contracts.Customer;
using PromoCodeFactory.WebHost.Models.Customer;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить данные всех клиентов
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var customers = (await _customerService.GetAllAsync(cancellationToken))
                .Select(c => _mapper.Map<CustomerShortResponse>(c)).ToList();

            return Ok(customers);
        }

        /// <summary>
        /// Получить данные клиента по id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            CustomerDto customer = await _customerService.GetAsync(id, cancellationToken);
            
            if(customer is null)
                return NotFound($"No Customer with Id {id} found");

            var customerResponse = _mapper.Map<CustomerResponse>(customer);
            return Ok(customerResponse);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateOrEditCustomerRequest request,
            CancellationToken cancellationToken)
        {
            var createOrEditCustomerDto = _mapper.Map<CreateOrEditCustomerDto>(request);
            var customerDto = await _customerService.CreateAsync(createOrEditCustomerDto, cancellationToken);
            var actionName = nameof(GetAsync);
            return CreatedAtAction(actionName, new {customerDto!.Id, cancellationToken}, customerDto);
        }

        /// <summary>
        /// Отредактировать клиента с id
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync(
            Guid id, 
            [FromBody] CreateOrEditCustomerRequest request, 
            CancellationToken cancellationToken)
        {
            var createOrEditCustomerDto = _mapper.Map<CreateOrEditCustomerDto>(request);
            if (!await _customerService.EditAsync(id, createOrEditCustomerDto, cancellationToken))
                return BadRequest($"Error updating Customer with Id {id}");

            return NoContent();
        }

        /// <summary>
        /// Удалить клиента с id
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (!await _customerService.DeleteAsync(id, cancellationToken))
                return BadRequest($"Error deleting Employee with Id {id}");

            return NoContent();
        }
    }
}