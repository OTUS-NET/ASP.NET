using System;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PartnerPromoCodeLimit:BaseEntity
    {
        public int Limit { get; set; }
        public required DateTime CreateDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public required DateTime EndDate { get; set; }
        public Guid PartnerId { get; set; }
        public virtual Partner Partner { get; set; }
    }
}