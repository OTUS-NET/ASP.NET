using PromoCodeFactory.Services.Contracts.PromoCode;

namespace PromoCodeFactory.Services.Abstractions;

public interface IPromoCodeService
{
    public Task<IEnumerable<PromoCodeShortDto>> GetAllAsync(CancellationToken cancellationToken);
    public Task<bool> GiveToCustomersWithPreferenceAsync(GivePromoCodeDto givePromoCodeDto, CancellationToken cancellationToken);
}