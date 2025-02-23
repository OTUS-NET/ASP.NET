namespace PromoCodeFactory.Services.Partners.Dto;

public class PartnerDto
{
    public Guid Id { get; set; }

    public bool IsActive { get; set; }
        
    public string Name { get; set; }

    public int NumberIssuedPromoCodes  { get; set; }

    public List<PartnerPromoCodeLimitDto> PartnerLimits { get; set; }
}