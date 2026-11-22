using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Abstractions;

public interface INotificationGateway
{
    /// <summary>
    /// Уведомление партнёру о созданном промокоде.
    /// </summary>
    Task PromoCodeCreated(PromoCode promoCode, CancellationToken ct);

    /// <summary>
    /// Уведомление партнёру о новом лимите промокодов.
    /// </summary>
    Task LimitCreated(PartnerPromoCodeLimit limit, CancellationToken ct);

    /// <summary>
    /// Уведомление партнёру об отмене лимита промокодов.
    /// </summary>
    Task LimitCanceled(PartnerPromoCodeLimit limit, CancellationToken ct);
}
