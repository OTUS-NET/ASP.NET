namespace PromoCodeFactory.Services.Contracts.Customer;

public class CustomerShortDto
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
}