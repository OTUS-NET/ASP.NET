using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Services.Abstractions;
using PromoCodeFactory.Services.Contracts.PromoCode;
using PromoCodeFactory.WebHost.Models.PromoCode;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController : ControllerBase
    {
        private readonly IPromoCodeService _promoCodeService;
        private readonly IMapper _mapper;

        public PromocodesController(IPromoCodeService promoCodeService, IMapper mapper)
        {
            _promoCodeService = promoCodeService;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var customers = (await _promoCodeService.GetAllAsync(cancellationToken))
                .Select(c => _mapper.Map<PromoCodeShortResponse>(c)).ToList();

            return Ok(customers);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(
            [FromBody] GivePromoCodeRequest request,
            CancellationToken cancellationToken)
        {
            var givePromoCodeDto = _mapper.Map<GivePromoCodeDto>(request);
            if (!await _promoCodeService.GiveToCustomersWithPreferenceAsync(givePromoCodeDto, cancellationToken))
                return BadRequest($"Error giving PromoCodes to Customers");

            return Ok();
        }
    }
}