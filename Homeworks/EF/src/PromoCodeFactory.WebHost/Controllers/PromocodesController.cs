using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IMapper _mapper;

        public PromocodesController(IRepository<PromoCode> promoCodeRepository,
                                     IRepository<Preference> preferenceRepository, 
                                      IMapper mapper)
        {
            _promoCodeRepository = promoCodeRepository;
            _preferenceRepository = preferenceRepository;
            _mapper = mapper;   
        }
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(CancellationToken cancellationToken)
        {
            var customers = (await _promoCodeRepository.GetAllAsync(cancellationToken, true))
                              .Select(c => _mapper.Map<PromoCodeShortResponse>(c)).ToList(); ;
            return Ok(customers);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request, CancellationToken cancellationToken)
        {
            if (!DateTime.TryParse(request.BeginDate, out var beginDate) ||
                !DateTime.TryParse(request.EndDate, out var endDate))
            {
                return BadRequest("Invalid date format. Please use a valid date string.");
            }

            // Проверяем существование предпочтения
            var preference = (await _preferenceRepository.GetAllAsync(cancellationToken))
                .Select(_mapper.Map<PreferenceShortResponse>).ToList()
                 .Where(p=>p.Name == request.Preference)
                  .FirstOrDefault();
            if (preference == null)
            {
                return NotFound($"Preference {request.Preference} not found.");
            }
            //Получаем список с данным предпочтением
            var preferenceFull = await _preferenceRepository.GetAsync(preference.Id);
            if (preferenceFull == null)
            {
                return NotFound($"Preference Details (Customers) {request.Preference} not found.");
            }
            var customers= preferenceFull.Customers;
            if (customers != null)
            {
                foreach (var customer in customers)
                {
                    var promoCode = new PromoCode
                    {
                        Code = request.PromoCode,
                        ServiceInfo = request.ServiceInfo,
                        BeginDate = beginDate,
                        EndDate = endDate,
                        PartnerName = request.PartnerName,
                        PreferenceId = preference.Id,
                        CustomerId = customer.Id
                    };
                    await _promoCodeRepository.AddAsync(promoCode);
                }
            }
            return Ok(customers);
        }
    }
}