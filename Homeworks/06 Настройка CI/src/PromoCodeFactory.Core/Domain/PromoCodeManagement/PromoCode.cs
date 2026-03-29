namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class PromoCode : BaseEntity
{
    public required string Code { get; set; }

    public required string ServiceInfo { get; set; }

    public required DateTimeOffset BeginDate { get; set; }

    public required DateTimeOffset EndDate { get; set; }

    public required Partner Partner { get; set; }

    public required Preference Preference { get; set; }

    public ICollection<CustomerPromoCode> CustomerPromoCodes { get; set; } = [];
}
