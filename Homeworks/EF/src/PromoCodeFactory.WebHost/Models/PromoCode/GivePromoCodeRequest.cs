namespace PromoCodeFactory.WebHost.Models.PromoCode
{
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; }

        public string PartnerName { get; set; }

        public string PromoCode { get; set; }

        public string Preference { get; set; }
        
        public string BeginDate { get; set; }

        public string EndDate { get; set; }
    }
}