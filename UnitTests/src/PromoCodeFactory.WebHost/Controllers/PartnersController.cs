using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Helpers;
using PromoCodeFactory.WebHost.Models.Request;
using PromoCodeFactory.WebHost.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Партнеры
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PartnersController(IPartnerRepository partnersRepository) :ControllerBase
    {
        /// <summary>
        /// Список партнеров
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PartnerResponse>>> GetPartnersAsync()
        {
            var partners = await partnersRepository.GetAllAsync();

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
        /// <summary>
        /// Получить партнерский лимит 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limitId"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}/limits/{limitId}")]
        public async Task<ActionResult<PartnerPromoCodeLimit>> GetPartnerLimitAsync(Guid id, Guid limitId)
        {
            var partner = await partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound(ErrorMessages.PartnerHasNotBeenFound());

            if (!partner.IsActive) return BadRequest(ErrorMessages.PartnerIsNotActive());
            var limit = partner.PartnerLimits.FirstOrDefault(x => x.Id == limitId);
            if (limit == null) return NotFound();

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
        /// <summary>
        /// Установить партнерские лимиты промокодов
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request)
        {
            var partner = await partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();

            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest(ErrorMessages.PartnerHasNotBeenFound());

            //Установка лимита партнеру
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x =>
                !x.CancelDate.HasValue);

            if (activeLimit != null)
            {
                //Если партнеру выставляется лимит, то мы 
                //должны обнулить количество промокодов, которые партнер выдал, если лимит закончился, 
                //то количество не обнуляется
                partner.NumberIssuedPromoCodes = 0;

                //При установке лимита нужно отключить предыдущий лимит
                activeLimit.CancelDate = DateTime.Now;
            }

            if (request.Limit <= 0)
                return BadRequest(ErrorMessages.LimitMustBeGreaterThanZero());

            var newLimit = new PartnerPromoCodeLimit()
            {
                Limit = request.Limit,
                Partner = partner,
                PartnerId = partner.Id,
                CreateDate = DateTime.Now,
                EndDate = request.EndDate
            };

            partner.PartnerLimits.Add(newLimit);

            await partnersRepository.UpdateAsync(id,partner);

            return CreatedAtAction(nameof(GetPartnerLimitAsync), new { id = partner.Id, limitId = newLimit.Id }, null);
        }
        /// <summary>
        /// Отменить лимит промо-кода Партнера
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/canceledLimits")]
        public async Task<IActionResult> CancelPartnerPromoCodeLimitAsync(Guid id)
        {
            var partner = await partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();

            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest(ErrorMessages.PartnerIsNotActive());

            //Отключение лимита
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x =>
                !x.CancelDate.HasValue);

            if (activeLimit != null)
            {
                activeLimit.CancelDate = DateTime.Now;
            }

            await partnersRepository.UpdateAsync(id,partner);

            return NoContent();
        }

    }
}
