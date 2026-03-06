using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

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
        private readonly IRepository<Partner> _partnersRepository;

        public PartnersController(IRepository<Partner> partnersRepository)
        {
            _partnersRepository = partnersRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<PartnerResponse>>> GetPartnersAsync()
        {
            var partners = await _partnersRepository.GetAllAsync();

            var response = partners.Select(x => new PartnerResponse()
            {
                Id = x.Id,
                Name = x.Name,
                NumberIssuedPromoCodes = x.NumberIssuedPromoCodes,
                IsActive = true,
                PartnerLimits = x.PartnerLimits
                    .Select(y => new PartnerPromoCodeLimitResponse()
                    {
                        Id = y.Id,
                        PartnerId = y.PartnerId,
                        Limit = y.Limit,
                        CreateDate = y.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        EndDate = y.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        CancelDate = y.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
                    }).ToList()
            });

            return Ok(response);
        }
        
        [HttpGet("{id}/limits/{limitId}")]
        public async Task<ActionResult<PartnerPromoCodeLimit>> GetPartnerLimitAsync(Guid id, Guid limitId)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();
            
            var limit = partner.PartnerLimits
                .FirstOrDefault(x => x.Id == limitId);

            var response = new PartnerPromoCodeLimitResponse()
            {
                Id = limit.Id,
                PartnerId = limit.PartnerId,
                Limit = limit.Limit,
                CreateDate = limit.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
                EndDate = limit.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
                CancelDate = limit.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
            };
            
            return Ok(response);
        }
        
        [HttpPost("{id}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            var validateResult = ValidatePartner(partner);
            if (validateResult != null)
                return validateResult;

            if (request.Limit <= 0)
                return BadRequest("Лимит должен быть больше 0");

            var activeLimit = partner.PartnerLimits.FirstOrDefault(x => !x.CancelDate.HasValue);
            if (activeLimit != null)
            {
                // Сбрасываем счётчик только если лимит ещё не истёк
                if (activeLimit.EndDate > DateTime.UtcNow)
                {
                    partner.NumberIssuedPromoCodes = 0;
                }

                activeLimit.CancelDate = DateTime.UtcNow;
            }

            var newLimit = new PartnerPromoCodeLimit
            {
                PartnerId = partner.Id,
                Partner = partner,
                Limit = request.Limit,
                CreateDate = DateTime.UtcNow,
                EndDate = request.EndDate
            };

            partner.PartnerLimits.Add(newLimit);

            await _partnersRepository.UpdateAsync(partner);

            return CreatedAtAction(nameof(GetPartnerLimitAsync), new {id = partner.Id, limitId = newLimit.Id}, null);
        }
        
        [HttpPost("{id}/canceledLimits")]
        public async Task<IActionResult> CancelPartnerPromoCodeLimitAsync(Guid id)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            var validateResult = ValidatePartner(partner);
            if (validateResult != null)
                return validateResult;

            //Отключение лимита
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x => 
                !x.CancelDate.HasValue);
            
            if (activeLimit != null)
            {
                activeLimit.CancelDate = DateTime.Now;
            }

            await _partnersRepository.UpdateAsync(partner);

            return NoContent();
        }

        private IActionResult? ValidatePartner(Partner? partner)
        {
            if (partner == null)
                return NotFound();

            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest("Данный партнер не активен");

            return null;
        }
    }
}