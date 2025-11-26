using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways
{
    public interface IPreferenceCacheGateway
    {
        Task<Preference> GetByIdAsync(Guid preferenceId);

        Task AddPreferenceAsync(Preference preference);
    }
}