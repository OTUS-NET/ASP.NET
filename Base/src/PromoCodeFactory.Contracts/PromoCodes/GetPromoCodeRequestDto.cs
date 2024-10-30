namespace PromoCodeFactory.Contracts.PromoCodes;

public record GetPromoCodeRequestDto
{
    public required Guid? PreferenceId { get; set; }
    
    public required string FromDate { get; set; }
    
    public required string ToDate { get; set; }
}