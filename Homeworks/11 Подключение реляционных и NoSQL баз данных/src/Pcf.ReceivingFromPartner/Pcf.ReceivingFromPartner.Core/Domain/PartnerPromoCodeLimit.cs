using Pcf.Common.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Domain;

public class PartnerPromoCodeLimit : BaseEntity
{
    public required Partner Partner { get; set; }

    public required DateTimeOffset EndAt { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? CanceledAt { get; set; }

    public required int Limit { get; set; }

    public int IssuedCount { get; set; }
}
