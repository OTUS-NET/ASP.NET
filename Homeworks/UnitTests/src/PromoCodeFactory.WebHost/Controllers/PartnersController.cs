using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Services.Partners.Abstractions;
using PromoCodeFactory.WebHost.Services.Partners.Dto;
using PromoCodeFactory.WebHost.Services.Partners.Exceptions;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Партнеры
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PartnersController
        : ControllerBase
    {
        private readonly IPartnersService _partnersService;
        private readonly IMapper _mapper;

        public PartnersController(IPartnersService partnersService, IMapper mapper)
        {
            _partnersService = partnersService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PartnerResponse>>> GetPartnersAsync()
        {
            var partners = await _partnersService.GetPartnersAsync();
            return Ok(_mapper.Map<List<PartnerResponse>>(partners));
        }
        
        [HttpGet("{id}/limits/{limitId}")]
        public async Task<ActionResult<PartnerPromoCodeLimit>> GetPartnerLimitAsync(Guid id, Guid limitId)
        {
            try
            {
                var limit = await _partnersService.GetPartnerLimitAsync(id, limitId);
                return Ok(_mapper.Map<PartnerPromoCodeLimit>(limit));
            }
            catch (PartnerNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpPost("{id}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request)
        {
            try
            {
                var dto = _mapper.Map<SetPartnerPromoCodeLimitDto>(request);
                var newLimit = await _partnersService.SetPartnerPromoCodeLimitAsync(id, dto);
                return CreatedAtAction(nameof(GetPartnerLimitAsync), new {id, limitId = newLimit.Id}, null);
            }
            catch (PartnerNotFoundException)
            {
                return NotFound();
            }
            catch (PartnerIsNotActiveException)
            {
                return BadRequest("Данный партнер не активен");
            }
            catch (IncorrectLimitException)
            {
                return BadRequest("Лимит должен быть больше 0");
            }
        }
        
        [HttpPost("{id}/canceledLimits")]
        public async Task<IActionResult> CancelPartnerPromoCodeLimitAsync(Guid id)
        {
            try
            {
                await _partnersService.CancelPartnerPromoCodeLimitAsync(id);
                return NoContent();
            }
            catch (PartnerNotFoundException)
            {
                return NotFound();
            }
            catch (PartnerIsNotActiveException)
            {
                return BadRequest("Данный партнер не активен");
            }
        }
    }
}