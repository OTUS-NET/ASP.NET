namespace PromoCodeFactory.WebHost.Models.PromoCodes;

public record PromoCodeShortResponse(
    Guid Id,
    string Code,
    string ServiceInfo,
    string BeginDate,
    string EndDate,
    string PartnerName);
