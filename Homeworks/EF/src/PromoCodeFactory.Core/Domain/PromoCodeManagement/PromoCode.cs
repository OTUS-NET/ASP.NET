using System;
using System.Runtime;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PromoCode
        : BaseEntity
    {
        public string Code { get; set; }

        public string ServiceInfo { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public string PartnerName => $"{PartnerManager?.FirstName ?? ""} {PartnerManager?.LastName ?? ""}";

        public Guid PartnerManagerId { get; set; }
        public virtual Employee PartnerManager { get; set; }

        public Guid PreferenceId { get; set; }
        public virtual Preference Preference { get; set; }

        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}