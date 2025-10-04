using Pcf.SharedLibrary.Models;
using System;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways
{
    public interface IPreferencesDirectoryGateway
    {
        Task<PreferenceResponse> GetPreferenceByIdAsync(Guid id);
    }
}
