namespace PromoCodeFactory.WebHost.Models.PromoCodes;

public record PromoCodeCreateRequest(
    string Code,
    string ServiceInfo,
    Guid PartnerId,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate,
    Guid PreferenceId);
