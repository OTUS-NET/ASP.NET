using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Abstractions;

public interface IGivingToCustomerGateway
{
    /// <summary>
    /// Передача информации о создании партнером промокода в микросервис по клиентам
    /// </summary>
    Task CreatePromoCode(PromoCode promoCode, CancellationToken ct);
}
