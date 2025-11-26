using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    public interface IPreferenceCacheGateway
    {
        Task<Preference> GetByIdAsync(Guid preferenceId);

        Task AddPreferenceAsync(Preference preference);
    }
}