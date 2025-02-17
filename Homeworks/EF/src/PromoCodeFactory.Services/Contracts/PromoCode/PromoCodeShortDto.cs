namespace PromoCodeFactory.Services.Contracts.PromoCode;

public class PromoCodeShortDto
{
    public Guid Id { get; set; }

    public required string Code { get; set; }

    public required string ServiceInfo { get; set; }

    public required string BeginDate { get; set; }

    public required string EndDate { get; set; }

    public required string PartnerName { get; set; }
}