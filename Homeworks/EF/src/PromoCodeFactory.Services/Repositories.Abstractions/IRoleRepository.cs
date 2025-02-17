using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Services.Repositories.Abstractions;

public interface IRoleRepository
{
    Task AddRangeIfNotExistsAsync(IEnumerable<Role> roles, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false);
}