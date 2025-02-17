namespace PromoCodeFactory.Services.Contracts.Customer;

public class CreateOrEditCustomerDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required List<Guid> PreferenceIds { get; set; }
}