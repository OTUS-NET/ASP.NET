using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Contracts.Preference;

namespace PromoCodeFactory.Services.Repositories.Abstractions;

public interface IPreferenceRepository
{
    Task<Preference?> GetAsync(Guid preferenceId, CancellationToken cancellationToken);
    Task<IEnumerable<Preference>> GetAllAsync(
        CancellationToken cancellationToken, 
        bool asNoTracking = false,
        PreferenceFilterDto? preferenceFilterDto = null);

    Task AddRangeIfNotExistsAsync(IEnumerable<Preference> preferences, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}