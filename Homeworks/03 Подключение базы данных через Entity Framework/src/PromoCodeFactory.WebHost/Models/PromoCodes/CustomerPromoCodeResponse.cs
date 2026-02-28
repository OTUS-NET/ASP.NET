namespace PromoCodeFactory.WebHost.Models.PromoCodes;

public record CustomerPromoCodeResponse(
    Guid Id,
    string Code,
    string ServiceInfo,
    string PartnerName,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate,
    Guid PartnerManagerId,
    Guid PreferenceId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? AppliedAt);
