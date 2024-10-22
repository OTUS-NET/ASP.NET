namespace PromoCodeFactory.Contracts.PromoCodes;

public record PromoCodeResponseDto
{
    public Guid Id { get; set; }
    
    public string Code { get; set; }
    
    public string ServiceInfo { get; set; }
    
    public DateTime BeginDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public string PartnerName { get; set; }
    
    public Guid PreferenceId { get; set; }
}