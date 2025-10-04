using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.SharedLibrary.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration
{
    public class PreferencesDirectoryGateway
        : IPreferencesDirectoryGateway
    {
        private readonly HttpClient _httpClient;

        public PreferencesDirectoryGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PreferenceResponse> GetPreferenceByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/v1/preferences/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PreferenceResponse>();
        }
    }
}
