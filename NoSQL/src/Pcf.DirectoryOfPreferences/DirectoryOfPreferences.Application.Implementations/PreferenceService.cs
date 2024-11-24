using AutoMapper;
using DirectoryOfPreferences.Application.Abstractions;
using DirectoryOfPreferences.Application.Abstractions.Exceptions;
using DirectoryOfPreferences.Application.Implementations.Base;
using DirectoryOfPreferences.Application.Models.Preference;
using DirectoryOfPreferences.Domain.Abstractions;
using DirectoryOfPreferences.Domain.Entity;

namespace DirectoryOfPreferences.Application.Implementations
{
    public class PreferenceService(IRepository<Preference, Guid> preferenceRepository, IMapper mapper) : BaseService, IPreferenceService
    {
        public async Task<PreferenceModel> AddPreferenceAsync(CreatePreferenceModel preferenceInfo, CancellationToken token = default)
        {
            Preference preference = new Preference(preferenceInfo.Name);
            preference = await preferenceRepository.AddAsync(preference, token)
                ?? throw new BadRequestException(FormatBadRequestErrorMessage(preference.Id, nameof(Preference)));
            return mapper.Map<PreferenceModel>(preference);
        }

        public async Task<IEnumerable<PreferenceModel>> GetAllPreferenceAsync(CancellationToken token = default)
        {
            var preferences = (await preferenceRepository.GetAllAsync(cancellationToken: token));
            return preferences.Select(mapper.Map<PreferenceModel>);
        }
    }
}
