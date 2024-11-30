using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration
{
    public class PreferenceGateway(HttpClient httpClient) : IPreferenceGateway
    {
        public async Task<IEnumerable<Preference>> GetPreferences()
        {
            using var response = await httpClient.GetAsync($"api/v1/Preferences");
            return await response.Content.ReadFromJsonAsync<IEnumerable<Preference>>();
        }
    }
}
