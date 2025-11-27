using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    public interface IPreferenceCacheGateway
    {
        Task<Preference> GetByIdAsync(Guid preferenceId);

        Task AddPreferenceAsync(Preference preference);
        Task<List<Preference>> GetAllAsync();
        Task AddPreferencesAsync(IEnumerable<Preference> preferences);
    }
}