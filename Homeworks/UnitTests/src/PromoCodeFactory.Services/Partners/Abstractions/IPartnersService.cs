using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Partners.Dto;

namespace PromoCodeFactory.Services.Partners.Abstractions;

public interface IPartnersService
{
    Task<List<PartnerDto>> GetPartnersAsync();
    Task<PartnerPromoCodeLimit> GetPartnerLimitAsync(Guid id, Guid limitId);
    Task<PartnerPromoCodeLimit> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitDto dto);
    Task CancelPartnerPromoCodeLimitAsync(Guid id);
}