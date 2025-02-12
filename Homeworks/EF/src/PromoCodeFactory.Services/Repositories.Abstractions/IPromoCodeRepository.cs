using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Services.Repositories.Abstractions;

public interface IPromoCodeRepository
{
    Task<IEnumerable<PromoCode>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false);
    Task AddAsync(PromoCode promoCode, CancellationToken cancellationToken);
}