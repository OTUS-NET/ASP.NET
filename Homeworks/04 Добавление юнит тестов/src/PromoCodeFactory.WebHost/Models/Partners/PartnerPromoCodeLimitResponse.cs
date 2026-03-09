namespace PromoCodeFactory.WebHost.Models.Partners;

public record PartnerPromoCodeLimitResponse(
    Guid Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset EndAt,
    DateTimeOffset? CanceledAt,
    int Limit);
