using PromoCodeFactory.WebHost.Models.Preferences;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Models.Customers;

public record CustomerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    IReadOnlyCollection<PreferenceShortResponse> Preferences,
    IReadOnlyCollection<PromoCodeShortResponse> PromoCodes);
