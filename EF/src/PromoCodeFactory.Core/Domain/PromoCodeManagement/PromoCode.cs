using System;
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
        public string PartnerName { get; set; }

        // Relations
        public Guid PartnerManagerId { get; set; }
        public Employee PartnerManager { get; set; }

        public Guid PreferenceId { get; set; }
        public Preference Preference { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }  
    }
}