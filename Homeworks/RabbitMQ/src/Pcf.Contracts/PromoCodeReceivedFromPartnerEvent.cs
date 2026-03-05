using System;

namespace Pcf.Contracts
{
    public class PromoCodeReceivedFromPartnerEvent
    {
        public Guid PromoCodeId { get; init; }
        public Guid PartnerId { get; init; }
        public required string PromoCode { get; init; }
        public Guid PreferenceId { get; init; }
        public DateTime BeginDate { get; init; }
        public DateTime EndDate { get; init; }
        public required string ServiceInfo { get; init; }
        public Guid? PartnerManagerId { get; init; }
    }
}

