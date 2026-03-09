namespace PromoCodeFactory.WebHost.Models.Partners;

public record PartnerResponse(
    Guid Id,
    string Name,
    int NumberIssuedPromoCodes,
    bool IsActive,
    Guid ManagerId,
    IReadOnlyCollection<PartnerPromoCodeLimitResponse> PartnerLimits);
