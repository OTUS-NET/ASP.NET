using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Utils;

namespace PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
        : ControllerBase
    {
        private readonly IRepository<Preference> _repo;

        public PreferencesController(IRepository<Preference> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Получить все предпочтения.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<EmployeeShortResponse>>> GetPreferences(CancellationToken token)
        {
            var preferences = await _repo.GetAllAsync(token);
            return Ok(preferences.ToResponseList());
        }
    }
}