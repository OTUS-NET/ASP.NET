using DirectoryOfPreferences.Application.Models.Preference;

namespace DirectoryOfPreferences.Application.Abstractions
{
    public interface IPreferenceService
    {
        Task<IEnumerable<PreferenceModel>> GetAllPreferenceAsync(CancellationToken token = default);
        Task<PreferenceModel> AddPreferenceAsync(CreatePreferenceModel preferenceInfo, CancellationToken token = default);
    }
}
