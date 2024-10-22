namespace PromoCodeFactory.Contracts.Customers;

public record PreferenceResponseDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
}