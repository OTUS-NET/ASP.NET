using PromoCodeFactory.Core.Domain.Administration;
using System;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PromoCode : BaseEntity
    {
        /// <summary>
        /// Previous
        /// </summary>
        //public string Code { get; set; }
        //public string ServiceInfo { get; set; }
        //public DateTime BeginDate { get; set; }
        //public DateTime EndDate { get; set; }
        //public string PartnerName { get; set; }
        //public virtual Employee PartnerManager { get; set; }
        //public virtual Preference Preference { get; set; }
        public string Code { get; set; }
        public string ServiceInfo { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PartnerName { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer Owner { get; set; }
        public Guid EmployeeId { get; set; }
        public virtual Employee PartnerManager { get; set; }
        public Guid PreferenceId { get; set; }
        public virtual Preference Preference { get; set; }
    }
}