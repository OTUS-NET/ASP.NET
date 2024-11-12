namespace PromoCodeFactory.Contracts.Customers;

public record CustomerPreferenceDto
{
    public required Guid CustomerId { get; set; }

    public required Guid PreferenceId { get; set; }
}