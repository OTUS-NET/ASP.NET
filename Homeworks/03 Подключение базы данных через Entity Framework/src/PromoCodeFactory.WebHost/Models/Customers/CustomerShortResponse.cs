namespace PromoCodeFactory.WebHost.Models.Customers;

public record CustomerShortResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);
