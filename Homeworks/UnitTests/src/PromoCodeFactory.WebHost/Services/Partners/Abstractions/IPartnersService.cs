using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Services.Partners.Dto;

namespace PromoCodeFactory.WebHost.Services.Partners.Abstractions;

public interface IPartnersService
{
    Task<List<PartnerDto>> GetPartnersAsync();
    Task<PartnerPromoCodeLimit> GetPartnerLimitAsync(Guid id, Guid limitId);
    Task<PartnerPromoCodeLimit> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitDto dto);
    Task CancelPartnerPromoCodeLimitAsync(Guid id);
}