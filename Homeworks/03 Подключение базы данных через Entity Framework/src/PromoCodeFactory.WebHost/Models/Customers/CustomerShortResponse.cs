using PromoCodeFactory.WebHost.Models.Preferences;

namespace PromoCodeFactory.WebHost.Models.Customers;

public record CustomerShortResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    IReadOnlyCollection<PreferenceShortResponse> Preferences);
