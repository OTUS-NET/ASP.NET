using PromoCodeFactory.Contracts.Preferences;

namespace PromoCodeFactory.Contracts.Customers;

public record CustomerResponseDto
{
    public Guid Id { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string Email { get; set; }
    
    public List<PreferenceResponseDto>? Preferences { get; set; }
}