namespace Pcf.GivingToCustomer.WebHost.Models.PromoCodes;

public record CreatePromoCodeRequest(
    Guid Id,
    string Code,
    string ServiceInfo,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate,
    Guid PartnerId,
    Guid PreferenceId);
