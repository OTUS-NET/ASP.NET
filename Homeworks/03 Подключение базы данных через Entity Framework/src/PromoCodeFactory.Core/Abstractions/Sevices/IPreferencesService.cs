using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Abstractions.Sevices;

public interface IPreferencesService
{
    Task<IEnumerable<Preference>> GetAsync(bool withIncludes = false, CancellationToken ct = default);
}
