using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
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

            await _httpClient.PostAsync("/PreferencesCache", jsonContent);
        }

        public async Task<Preference> GetByIdAsync(Guid preferenceId)
        {
            var response = await _httpClient.GetAsync($"/PreferencesCache/{preferenceId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Preference>(stream, serializerOptions);
        }

        public async Task<List<Preference>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("/PreferencesCache");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<Preference>>(stream, serializerOptions);
        }

        public async Task AddPreferencesAsync(IEnumerable<Preference> preferences)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(preferences), System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("/PreferencesCache/list", jsonContent);
        }
    }
}