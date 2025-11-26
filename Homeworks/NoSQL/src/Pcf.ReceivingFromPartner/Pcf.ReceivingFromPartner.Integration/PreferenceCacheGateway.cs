using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class PreferenceCacheGateway
        : IPreferenceCacheGateway
    {
        private readonly HttpClient _httpClient;

        public PreferenceCacheGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddPreferenceAsync(Preference preference)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(preference), System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("preferences", jsonContent);
        }

        public async Task<Preference> GetByIdAsync(Guid preferenceId)
        {
            var response = await _httpClient.GetAsync($"preferences/{preferenceId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Preference>(jsonString);
        }
    }
}