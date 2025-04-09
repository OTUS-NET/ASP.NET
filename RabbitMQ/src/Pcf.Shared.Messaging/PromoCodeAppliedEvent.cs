namespace Pcf.Shared.Messaging;

public class PromoCodeAppliedEvent 
{
    public string? Code { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? ServiceInfo { get; set; }
    public Guid PreferenceId { get; set; }
    public Guid? PartnerManagerId { get; set; }
    public Guid PartnerId { get; set; }
}
