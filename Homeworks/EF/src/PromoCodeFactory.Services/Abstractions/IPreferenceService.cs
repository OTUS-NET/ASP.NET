using PromoCodeFactory.Services.Contracts.Preference;

namespace PromoCodeFactory.Services.Abstractions;

public interface IPreferenceService
{
    Task<IEnumerable<PreferenceShortDto>> GetAllAsync(CancellationToken cancellationToken);
}