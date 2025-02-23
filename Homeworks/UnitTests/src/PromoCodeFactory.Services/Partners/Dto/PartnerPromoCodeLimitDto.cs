using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Services.Partners.Dto;

public class PartnerPromoCodeLimitDto
{
    public Guid Id { get; set; }

    public Guid PartnerId { get; set; }

    public virtual Partner Partner { get; set; }
        
    public string CreateDate { get; set; }

    public string? CancelDate { get; set; }

    public string EndDate { get; set; }

    public int Limit { get; set; }
}