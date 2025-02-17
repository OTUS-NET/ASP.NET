using AutoMapper;
using PromoCodeFactory.Services.Abstractions;
using PromoCodeFactory.Services.Contracts.Preference;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.Services.Implementations;

public class PreferenceService : IPreferenceService
{
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly IMapper _mapper;

    public PreferenceService(IPreferenceRepository preferenceRepository, IMapper mapper)
    {
        _preferenceRepository = preferenceRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PreferenceShortDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var preferences = await _preferenceRepository.GetAllAsync(cancellationToken, true);
        return preferences.Select(c => _mapper.Map<PreferenceShortDto>(c));
    }
}