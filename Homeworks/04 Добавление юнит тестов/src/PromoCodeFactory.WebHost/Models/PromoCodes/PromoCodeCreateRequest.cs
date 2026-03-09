namespace PromoCodeFactory.WebHost.Models.PromoCodes;

public record PromoCodeCreateRequest(
    string Code,
    string ServiceInfo,
    string PartnerName,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate,
    Guid PartnerManagerId,
    Guid PreferenceId);
