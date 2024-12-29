namespace PromoCodeFactory.Contracts.Preferences;

public record PreferenceResponseDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
}