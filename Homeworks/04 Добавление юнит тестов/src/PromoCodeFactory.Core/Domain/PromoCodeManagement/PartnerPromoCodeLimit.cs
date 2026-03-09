namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class PartnerPromoCodeLimit : BaseEntity
{
    public required Partner Partner { get; set; }

    public required DateTimeOffset EndAt { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? CanceledAt { get; set; }

    public required int Limit { get; set; }
}
