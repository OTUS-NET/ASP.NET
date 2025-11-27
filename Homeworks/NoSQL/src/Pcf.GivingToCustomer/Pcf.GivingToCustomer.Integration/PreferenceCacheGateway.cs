using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration
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

        public async Task<List<Preference>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Preference>>(jsonString);
        }

        public async Task AddPreferencesAsync(IEnumerable<Preference> preferences)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(preferences), System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("list", jsonContent);
        }
    }
}