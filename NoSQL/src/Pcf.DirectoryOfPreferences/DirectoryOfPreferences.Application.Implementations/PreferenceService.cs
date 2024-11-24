using DirectoryOfPreferences.Application.Abstractions;
using DirectoryOfPreferences.Application.Models.Preference;

namespace DirectoryOfPreferences.Application.Implementations
{
    public class PreferenceService : IPreferenceService
    {
        public async Task<PreferenceModel> AddPreferenceAsync(CreatePreferenceModel preferenceInfo, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PreferenceModel>> GetAllPreferenceAsync(CancellationToken token = default)
        {
            return new List<PreferenceModel>() { new PreferenceModel() {Id = Guid.NewGuid(),Name = "Theatre" } };
        }
    }
}
