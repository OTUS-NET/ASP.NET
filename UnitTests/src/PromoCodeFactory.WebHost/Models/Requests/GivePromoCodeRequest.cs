using System;

namespace PromoCodeFactory.WebHost.Models.Requests
{
    public class GivePromoCodeRequest
    {
        /// <summary>
        /// Previous
        /// </summary>
        //public string ServiceInfo { get; set; }

        //public string PartnerName { get; set; }

        //public string PromoCode { get; set; }

        //public string Preference { get; set; }

        public string Code { get; set; }
        public string ServiceInfo { get; set; }
        public string PartnerName { get; set; }
        public int BeforeStarts { get; set; }
        public int HowLongDay { get; set; }
        public Guid CustomerId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid PreferenceId { get; set; }

    }
}