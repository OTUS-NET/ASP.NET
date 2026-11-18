using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Sevices;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Services;

public class PreferencesService(IRepository<Preference> _preferenceRepository) : IPreferencesService
{
    public async Task<IEnumerable<Preference>> GetAsync(bool withIncludes = false, CancellationToken ct = default)
    {
        return await _preferenceRepository.GetAll(withIncludes, ct);
    }
}
