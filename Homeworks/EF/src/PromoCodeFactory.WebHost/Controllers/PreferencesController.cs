using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
        : ControllerBase
    {
        private IRepository<Preference> _preferenceRepository;
        public PreferencesController
            (
            IRepository<Preference> preferenceRepository
            ) 
        {
            //_customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Получить список кратких описаний всех предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PreferenceResponse>>> GetPreferencesAsync()
        {
            var dbResult = await _preferenceRepository.GetAllAsync();

            var result = dbResult.Select(x => new PreferenceResponse()
            {
                Name = x.Name,
                Id = x.Id
            });

            return Ok(result);
        }

        /// <summary>
        /// Получить предпочтение по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType<PreferenceResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<PreferenceResponse>> GetPreferenceAsync(Guid id)
        {
            //Добавить получение предпочтения вместе с выданными ему промомкодами
            var dbResult = await _preferenceRepository.GetByIdAsync(id);

            if (dbResult == null)
                return NotFound();

            //Тут только конверсия
            var response = new PreferenceResponse()
            {
                Name = dbResult.Name,
                Id = dbResult.Id
            };

            return Ok(response);

        }

        /// <summary>
        /// Создать предпочтение
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        [ProducesResponseType<PreferenceResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePreferenceAsync(CreateOrEditPreferenceRequest request)
        {
            if (!ValidatePreferenceRequest(request))
                return BadRequest();

            //Добавить создание нового предпочтения
            var newPreference = new Preference()
            {
               Name = request.Name
            };

            var dbResult = await _preferenceRepository.CreateAsync(newPreference);
            if (dbResult == null)
                return BadRequest();

            var response = new PreferenceResponse()
            {
                Name = dbResult.Name,
                Id= dbResult.Id
            };
            return Created("api/v1/Preferences", response);
        }

        /// <summary>
        /// Редактировать предпочтение
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditPreferenceAsync(Guid id, CreateOrEditPreferenceRequest request)
        {
            if (!ValidatePreferenceRequest(request))
                return BadRequest();

            var preference = new Preference()
            {
                Id = id,
                Name = request.Name
            };  

            var dbResult = await _preferenceRepository.UpdateAsync(preference);
            return dbResult != null ? NoContent() : BadRequest();
        }

        /// <summary>
        /// Удалить предпочтение
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePreferenceAsync(Guid id)
        {
            var dbResult = await _preferenceRepository.DeleteAsync(id);
            return dbResult == Guid.Empty ? BadRequest() : NoContent();
        }

        /// <summary>
        /// Валидация запроса на создание клиента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool ValidatePreferenceRequest(CreateOrEditPreferenceRequest request)
        {
            //TODO
            //Заглушка
            return true;
        }
    }
}