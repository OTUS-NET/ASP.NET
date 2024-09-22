using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;

namespace PromoCodeFactory.WebHost.Models.Request
{
    public class GivePromoCodeRequest
    {
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
