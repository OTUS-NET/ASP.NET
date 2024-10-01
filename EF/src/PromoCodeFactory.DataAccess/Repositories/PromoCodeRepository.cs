using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class PromoCodeRepository : EfCoreRepository<PromoCode>
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<PromoCodeRepository> _logger;

        public PromoCodeRepository(DatabaseContext dbContext, ILogger<PromoCodeRepository> logger) : base(dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task CreateAsync(PromoCode promoCode, CancellationToken token)
        {
            var preferenceId = await _dbContext.Preferences.Where(p => p.Name == promoCode.Preference.Name).Select(p => p.Id).SingleOrDefaultAsync(token);
            var employeeId = await _dbContext.Employees.Where(e => e.FirstName == promoCode.PartnerName).Select(p => p.Id).SingleOrDefaultAsync(token);
            promoCode.PreferenceId = preferenceId;
            promoCode.PartnerManagerId = employeeId;
            await _dbContext.PromoCodes.AddAsync(promoCode, token);
            await _dbContext.SaveChangesAsync(token);
        }
    }
}
