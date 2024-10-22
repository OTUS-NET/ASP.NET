namespace PromoCodeFactory.Contracts.Customers;

public record CustomerResponseDto
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public List<PreferenceResponseDto> Preferences { get; set; }
}