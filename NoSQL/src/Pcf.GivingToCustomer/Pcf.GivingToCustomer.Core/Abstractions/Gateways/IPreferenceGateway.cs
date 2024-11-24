using Pcf.GivingToCustomer.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways
{
    public interface IPreferenceGateway
    {
        Task<IEnumerable<Preference>> GetPreferences();
    }
}
