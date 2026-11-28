namespace Pcf.Common.Contracts.GivingToCustomer;

/// <summary>
/// Команда выдать промокод клиентам с указанным предпочтением.
/// </summary>
public record CreatePromoCodeMessage(
    Guid Id,
    string Code,
    string ServiceInfo,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate,
    Guid PartnerId,
    Guid PreferenceId);
