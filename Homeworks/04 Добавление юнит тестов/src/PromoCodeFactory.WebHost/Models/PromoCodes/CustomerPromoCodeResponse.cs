namespace PromoCodeFactory.WebHost.Models.PromoCodes;

public record CustomerPromoCodeResponse(
    Guid Id,
    string Code,
    string ServiceInfo,
    Guid PartnerId,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate,
    Guid PreferenceId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? AppliedAt);
