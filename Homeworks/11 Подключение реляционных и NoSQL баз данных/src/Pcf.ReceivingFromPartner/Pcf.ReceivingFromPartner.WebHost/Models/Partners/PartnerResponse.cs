namespace Pcf.ReceivingFromPartner.WebHost.Models.Partners;

public record PartnerResponse(
    Guid Id,
    string Name,
    bool IsActive,
    Guid ManagerId,
    IReadOnlyCollection<PartnerPromoCodeLimitResponse> PartnerLimits);
