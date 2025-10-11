namespace Pcf.SharedLibrary.Events
{
    public record PromoCodeIssuedEvent
    {
        public Guid PromoCodeId { get; init; }
        public required string Code { get; init; }
        public string? ServiceInfo { get; init; }
        public DateTime BeginDate { get; init; }
        public DateTime EndDate { get; init; }
        public Guid PartnerId { get; init; }
        public Guid? PartnerManagerId { get; init; }
        public Guid PreferenceId { get; init; }
        public DateTime IssuedAt { get; init; } = DateTime.UtcNow;
    }
}
