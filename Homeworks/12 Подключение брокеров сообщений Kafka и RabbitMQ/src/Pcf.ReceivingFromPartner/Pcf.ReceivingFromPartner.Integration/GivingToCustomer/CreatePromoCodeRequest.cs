namespace Pcf.ReceivingFromPartner.Integration.GivingToCustomer;

internal class CreatePromoCodeRequest
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string ServiceInfo { get; set; }
    public required DateTimeOffset BeginDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
    public required Guid PartnerId { get; set; }
    public required Guid PreferenceId { get; set; }
}
