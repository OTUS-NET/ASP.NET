namespace PromoCodeFactory.Services.Contracts.PromoCode;

public class GivePromoCodeDto
{
    public required string ServiceInfo { get; set; }

    public required string PartnerName { get; set; }

    public required string PromoCode { get; set; }

    public required string Preference { get; set; }
    
    public DateTime BeginDate { get; set; }

    public DateTime EndDate { get; set; }
}