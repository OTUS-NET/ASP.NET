using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.WebHost.Models;
using Pcf.ReceivingFromPartner.WebHost.Mappers;

namespace Pcf.ReceivingFromPartner.WebHost.Controllers
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
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly INotificationGateway _notificationGateway;
        private readonly IGivingPromoCodeToCustomerGateway _givingPromoCodeToCustomerGateway;
        private readonly IAdministrationGateway _administrationGateway;

        public PartnersController(IRepository<Partner> partnersRepository,
            IRepository<Preference> preferencesRepository,
            INotificationGateway notificationGateway,
            IGivingPromoCodeToCustomerGateway givingPromoCodeToCustomerGateway,
            IAdministrationGateway administrationGateway)
        {
            _partnersRepository = partnersRepository;
            _preferencesRepository = preferencesRepository;
            _notificationGateway = notificationGateway;
            _givingPromoCodeToCustomerGateway = givingPromoCodeToCustomerGateway;
            _administrationGateway = administrationGateway;
        }

        /// <summary>
        /// Получить список партнеров
        /// </summary>
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

        /// <summary>
        /// Получить информацию партнере
        /// </summary>
        /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<List<PartnerResponse>>> GetPartnersAsync(Guid id)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
            {
                return NotFound();
            }

            var response = new PartnerResponse()
            {
                Id = partner.Id,
                Name = partner.Name,
                NumberIssuedPromoCodes = partner.NumberIssuedPromoCodes,
                IsActive = true,
                PartnerLimits = partner.PartnerLimits
                    .Select(y => new PartnerPromoCodeLimitResponse()
                    {
                        Id = y.Id,
                        PartnerId = y.PartnerId,
                        Limit = y.Limit,
                        CreateDate = y.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        EndDate = y.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        CancelDate = y.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
                    }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Установить лимит на промокоды для партнера
        /// </summary>
        [HttpPost("{id:guid}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();

            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest("Данный партнер не активен");

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
                return BadRequest("Лимит должен быть больше 0");

            var newLimit = new PartnerPromoCodeLimit()
            {
                Limit = request.Limit,
                Partner = partner,
                PartnerId = partner.Id,
                CreateDate = DateTime.Now,
                EndDate = request.EndDate
            };

            partner.PartnerLimits.Add(newLimit);

            await _partnersRepository.UpdateAsync(partner);

            await _notificationGateway
                .SendNotificationToPartnerAsync(partner.Id, "Вам установлен лимит на отправку промокодов...");

            return CreatedAtAction(nameof(GetPartnerLimitAsync), new { id = partner.Id, limitId = newLimit.Id }, null);
        }

        /// <summary>
        /// Получить лимит на промокоды для партнера
        /// </summary>
        /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
        /// <param name="limitId">Id лимита партнера, например: <example>93f3a79d-e9f9-47e6-98bb-1f618db43230</example></param>
        [HttpGet("{id:guid}/limits/{limitId:guid}")]
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

        /// <summary>
        /// Отменить лимит на промокоды для партнера
        /// </summary>
        /// <param name="id">Id партнера, например: <example>0da65561-cf56-4942-bff2-22f50cf70d43</example></param>
        [HttpPost("{id:guid}/canceledLimits")]
        public async Task<IActionResult> CancelPartnerPromoCodeLimitAsync(Guid id)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();

            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest("Данный партнер не активен");

            //Отключение лимита
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x =>
                !x.CancelDate.HasValue);

            if (activeLimit != null)
            {
                activeLimit.CancelDate = DateTime.Now;
            }

            await _partnersRepository.UpdateAsync(partner);

            //Отправляем уведомление
            await _notificationGateway
                .SendNotificationToPartnerAsync(partner.Id, "Ваш лимит на отправку промокодов отменен...");

            return NoContent();
        }

        /// <summary>
        /// Получить промокод партнера по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}/promocodes")]
        public async Task<IActionResult> GetPartnerPromoCodesAsync(Guid id)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
            {
                return NotFound("Партнер не найден");
            }

            var response = partner.PromoCodes
                .Select(x => new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    Code = x.Code,
                    BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                    PartnerName = x.Partner.Name,
                    PartnerId = x.PartnerId,
                    ServiceInfo = x.ServiceInfo
                }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получить промокод партнера по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}/promocodes/{promoCodeId:guid}")]
        public async Task<IActionResult> GetPartnerPromoCodeAsync(Guid id, Guid promoCodeId)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
            {
                return NotFound("Партнер не найден");
            }

            var promoCode = partner.PromoCodes.FirstOrDefault(x => x.Id == promoCodeId);

            if (promoCode == null)
            {
                return NotFound("Партнер не найден");
            }

            var response = new PromoCodeShortResponse()
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
                PartnerName = promoCode.Partner.Name,
                PartnerId = promoCode.PartnerId,
                ServiceInfo = promoCode.ServiceInfo
            };

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод от партнера 
        /// </summary>
        /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
        /// <param name="request">Данные запроса/example></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/promocodes")]
        public async Task<IActionResult> ReceivePromoCodeFromPartnerWithPreferenceAsync(Guid id,
            ReceivingPromoCodeRequest request)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
            {
                return BadRequest("Партнер не найден");
            }

            var activeLimit = partner.PartnerLimits.FirstOrDefault(x
                => !x.CancelDate.HasValue && x.EndDate > DateTime.Now);

            if (activeLimit == null)
            {
                return BadRequest("Нет доступного лимита на предоставление промокодов");
            }

            if (partner.NumberIssuedPromoCodes + 1 > activeLimit.Limit)
            {
                return BadRequest("Лимит на выдачу промокодов превышен");
            }

            if (partner.PromoCodes.Any(x => x.Code == request.PromoCode))
            {
                return BadRequest("Данный промокод уже был выдан ранее");
            }

            //Получаем предпочтение по имени
            var preference = await _preferencesRepository.GetByIdAsync(request.PreferenceId);

            if (preference == null)
            {
                return BadRequest("Предпочтение не найдено");
            }

            PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preference, partner);
            partner.PromoCodes.Add(promoCode);
            partner.NumberIssuedPromoCodes++;

            await _partnersRepository.UpdateAsync(partner);

            //TODO: Чтобы информация о том, что промокод был выдан парнером была отправлена
            //в микросервис рассылки клиентам нужно либо вызвать его API, либо отправить событие в очередь
            await _givingPromoCodeToCustomerGateway.GivePromoCodeToCustomer(promoCode);

            //TODO: Чтобы информация о том, что промокод был выдан парнером была отправлена
            //в микросервис администрирования нужно либо вызвать его API, либо отправить событие в очередь

            if (request.PartnerManagerId.HasValue)
            {
                await _administrationGateway.NotifyAdminAboutPartnerManagerPromoCode(request.PartnerManagerId.Value);
            }

            return CreatedAtAction(nameof(GetPartnerPromoCodeAsync),
                new { id = partner.Id, promoCodeId = promoCode.Id }, null);
        }
    }
}