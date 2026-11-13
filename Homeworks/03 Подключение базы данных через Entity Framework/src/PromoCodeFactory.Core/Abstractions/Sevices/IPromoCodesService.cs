using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Abstractions.Sevices;

public interface IPromoCodesService
{
    Task<IEnumerable<PromoCode>> GetAsync(bool withIncludes = false, CancellationToken ct = default);
    Task<PromoCode?> GetByIdAsync(Guid id, bool withIncludes = false, CancellationToken ct = default);
    Task<PromoCode> CreateAsync(PromoCode promoCode, CancellationToken ct = default);
    Task ApplyPromoCodeAsync(Guid promoCodeId, Guid customerId, CancellationToken ct = default);
}
