namespace PromoCodeFactory.WebHost.Models.PromoCodes;

public record PromoCodeShortResponse(
    Guid Id,
    string Code,
    string ServiceInfo,
    Guid PartnerId,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate,
    Guid PreferenceId);
