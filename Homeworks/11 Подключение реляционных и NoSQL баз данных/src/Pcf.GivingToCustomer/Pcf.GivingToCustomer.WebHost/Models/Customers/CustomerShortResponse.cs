using Pcf.GivingToCustomer.WebHost.Models.Preferences;

namespace Pcf.GivingToCustomer.WebHost.Models.Customers;

public record CustomerShortResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    IReadOnlyCollection<PreferenceShortResponse> Preferences);
