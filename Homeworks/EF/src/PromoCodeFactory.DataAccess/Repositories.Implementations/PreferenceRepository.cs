using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Contracts.Preference;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.DataAccess.Repositories.Implementations;

public class PreferenceRepository : EfRepository<Preference, Guid>, IPreferenceRepository
{
    public PreferenceRepository(DatabaseContext databaseContext) : base(databaseContext)
    {
    }

    public async Task<IEnumerable<Preference>> GetAllAsync(CancellationToken cancellationToken,
        bool asNoTracking = false,
        PreferenceFilterDto preferenceFilterDto = null)
    {
        var query = GetAll(asNoTracking);

        if (preferenceFilterDto?.Names is { Count: > 0 })
        {
            query = query.Where(p => preferenceFilterDto.Names.Contains(p.Name));
        }

        return await query.ToListAsync(cancellationToken);
    }
}